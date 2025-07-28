using System.ClientModel;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Files;

namespace DevExpress.AI.Samples.Blazor {
#pragma warning disable OPENAI001
    public class AIAssistantManager {
        readonly AssistantClient assistantClient;
        readonly OpenAIFileClient fileClient;
        readonly string deployment;

        public AIAssistantManager(OpenAIClient client, string deployment) {
            assistantClient = client.GetAssistantClient();
            fileClient = client.GetOpenAIFileClient();
            this.deployment = deployment;
        }

        public async Task<(string assistantId, string threadId, string fileId)> CreateAssistantAsync(Stream data, string fileName, string instructions, bool useFileSearchTool = true, CancellationToken ct = default) {
            data.Position = 0;

            ClientResult<OpenAIFile> fileResponse = await fileClient.UploadFileAsync(data, fileName, FileUploadPurpose.Assistants, ct);
            var file = fileResponse.Value;

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
            var assistant = assistantResponse.Value;
            ClientResult<AssistantThread> threadResponse = await assistantClient.CreateThreadAsync(cancellationToken: ct);
            var thread = threadResponse.Value;

            return (assistant.Id, thread.Id, file.Id);
        }

        public async Task CleanUpAssistantAsync(string? assistantId, string? threadId, string? fileId) {
            try{
                if(!string.IsNullOrEmpty(assistantId)){
                    await assistantClient.DeleteAssistantAsync(assistantId);
                }

                if(!string.IsNullOrEmpty(threadId)){
                    await assistantClient.DeleteThreadAsync(threadId);
                }

                if(!string.IsNullOrEmpty(fileId)){
                    await fileClient.DeleteFileAsync(fileId);
                }
            }
            catch{}
        }
    }
#pragma warning restore OPENAI001
}
