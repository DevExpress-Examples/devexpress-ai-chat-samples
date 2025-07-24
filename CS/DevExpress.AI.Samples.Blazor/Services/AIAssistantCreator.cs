using System.ClientModel;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Files;

namespace DevExpress.AI.Samples.Blazor {
#pragma warning disable OPENAI001
    public class AIAssistantCreator : IDisposable {
        readonly AssistantClient assistantClient;
        readonly OpenAIFileClient fileClient;
        readonly string deployment;
        AssistantThread thread;
        Assistant assistant;
        OpenAIFile file;

        public AIAssistantCreator(OpenAIClient client, string deployment) {
            assistantClient = client.GetAssistantClient();
            fileClient = client.GetOpenAIFileClient();
            this.deployment = deployment;
        }

        public async Task<(string assistantId, string threadId)> CreateAssistantAsync(Stream data, string fileName, string instructions, bool useFileSearchTool = true, CancellationToken ct = default) {
            data.Position = 0;

            ClientResult<OpenAIFile> fileResponse = await fileClient.UploadFileAsync(data, fileName, FileUploadPurpose.Assistants, ct);
            file = fileResponse.Value;

            var resources = new ToolResources() {
                CodeInterpreter = new CodeInterpreterToolResources(),
                FileSearch = useFileSearchTool ? new FileSearchToolResources() : null
            };
            resources.FileSearch?.NewVectorStores.Add(new VectorStoreCreationHelper([file.Id]));
            resources.CodeInterpreter.FileIds.Add(file.Id);

            AssistantCreationOptions assistantCreationOptions = new AssistantCreationOptions() {
                Name = Guid.NewGuid().ToString(),
                Instructions = instructions,
                ToolResources = resources
            };
            assistantCreationOptions.Tools.Add(new CodeInterpreterToolDefinition());
            if (useFileSearchTool) {
                assistantCreationOptions.Tools.Add(new FileSearchToolDefinition());
            }

            ClientResult<Assistant> assistantResponse = await assistantClient.CreateAssistantAsync(deployment, assistantCreationOptions, ct);
            assistant = assistantResponse.Value;
            ClientResult<AssistantThread> threadResponse = await assistantClient.CreateThreadAsync(cancellationToken: ct);
            thread = threadResponse.Value;

            return (assistantResponse.Value.Id, threadResponse.Value.Id);
        }

        public void Dispose() {
            Console.WriteLine("begin dispose");
            try {
                if(assistant != null){
                    Console.WriteLine("assistant not null");
                    assistantClient?.DeleteAssistant(assistant.Id);
                    assistantClient?.DeleteThread(thread.Id);
                    fileClient?.DeleteFile(file.Id);
                    assistant = null;
                    thread = null;
                    file = null;
                    Console.WriteLine("finish dispose");
                }
            } catch {}
        }
    }
#pragma warning restore OPENAI001
}
