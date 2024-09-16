<!-- default badges list -->
![](https://img.shields.io/endpoint?url=https://codecentral.devexpress.com/api/v1/VersionRange/851207927/24.2.1%2B)
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/T1251539)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
[![](https://img.shields.io/badge/💬_Leave_Feedback-feecdd?style=flat-square)](#does-this-example-address-your-development-requirementsobjectives)
<!-- default badges end -->

# Blazor AI Chat - How to add the DevExpress Blazor AI Chat component to your next Blazor, MAUI, WPF, and WinForms application

The DevExpress Blazor AI Chat component (DxAIChat) allows you to incorporate AI-powered interactions into any Blazor/MAUI/WPF/WinForms application. Our AI Chat component ships with a variety of high impact features, including:

* [Customizable appearance of messages and empty message area](#customize-appearance-of-messages-and-empty-message-area)
* [Text or markdown response](#text-or-markdown-response)
* [Manual message processing](#manual-message-processing)
* [Streaming response](#streaming-response)

## Implementation Details

This example adds a `DxAIChat` to a Blazor application, customizes its settings, and integrates it into WinForms, WPF, and .NET MAUI applications.

### Register AI Service

Add the following code to the _Program.cs_ file to register the AI Chat service in your application:

```cs
using DevExpress.AIIntegration;

string azureOpenAIEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
string azureOpenAIKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
...
builder.Services.AddDevExpressAI((config) => {
    var client = new AzureOpenAIClient(
        new Uri(azureOpenAIEndpoint),
        new AzureKeyCredential(azureOpenAIKey));
    config.RegisterChatClientOpenAIService(client, "gpt4o");
});
```

File to review: [Program.cs](./CS/DevExpress.AI.Samples.Blazor/Program.cs)

### Add DxAIChat component to a Blazor Application

Add a `<DxAIChat>…</DxAIChat>` markup to a .razor file:

```razor
@using DevExpress.AIIntegration.Blazor.Chat
@using AIIntegration.Services.Chat;

<DxAIChat CssClass="my-chat" />
```

```css
.my-chat {
    width: 700px;
    margin: 20px;
}
```

![AIChat](AIChat.png)


File to review: [Chat.razor](./CS/DevExpress.AI.Samples.Blazor/Components/Pages/Chat.razor)

### Customize message appearance and empty message area

`DxAIChat` component includes the following message customization properties:

* `MessageTemplate` - specifies the template used for message bubbles.
* `MessageContentTemplate` - specifies the template used for message bubble content.
* `EmptyMessageAreaTemplate` - specifies the template used for the empty message area.

```razor
<DxAIChat CssClass="my-chat">
    <EmptyMessageAreaTemplate>
        <div class="my-chat-ui-description">
            AI Assistant is ready to answer your questions.
        </div>
    </EmptyMessageAreaTemplate>
    <MessageTemplate>
        <div class="@GetMessageClasses(context)">
            @if(context.Typing) {
                <span>Loading...</span>
            } else {
                <div class="my-chat-content">
                    @context.Content
                </div>
            }
        </div>
    </MessageTemplate>
</DxAIChat>
```

File to review: [Chat-CustomMessage.razor](./CS/DevExpress.AI.Samples.Blazor/Components/Pages/Chat-CustomMessage.razor), [Chat-CustomEmptyState.razor](./CS/DevExpress.AI.Samples.Blazor/Components/Pages/Chat-CustomEmptyState.razor)

### Text or markdown response

The AI service uses plain text as the default response format.

To display rich formatted messages, set the `RenderMode` property to `Markdown`. Use a markdown processor to convert response content to HTML code.

```razor
@using Markdig;

<DxAIChat CssClass="my-chat" RenderMode="AnswerRenderMode.Markdown">
    <MessageContentTemplate>
        <div class="my-chat-content">
            @ToHtml(context.Content)
        </div>
    </MessageContentTemplate>
</DxAIChat>

@code {
    MarkupString ToHtml(string text) {
        return (MarkupString)Markdown.ToHtml(text);
    }
}
```

### Manual message processing

When a user sends a message to the chat, the `MessageSent` event fires. Handle the event to manually process this action. 
You can use the `Content` event argument to access user input and call the `SendMessage` method to send another message to the chat.

```razor
<DxAIChat CssClass="my-chat" MessageSent="MessageSent" />

@code {
    void MessageSent(MessageSentEventArgs args) {
        var message = new Message(MessageRole.Assistant, $"Processed: {args.Content}");
        args.SendMessage(message);
    }
}
```

File to review: [Chat-MessageSent.razor](./CS/DevExpress.AI.Samples.Blazor/Components/Pages/Chat-MessageSent.razor)


### Streaming response

After a user sends a request, the AI client generates and sends the entire response back. This operation may be time consuming. To make the chat appear more responsive, set the `UseStreaming` property to `true`. In this instance, the AI client transmits parts of the response as it becomes available and the chat component adds these parts to the display message.

```razor
<DxAIChat CssClass="my-chat" UseStreaming="true" />
```

File to review: [Chat-Streaming.razor](./CS/DevExpress.AI.Samples.Blazor/Components/Pages/Chat-Streaming.razor)

### Compatibility with OpenAI assistants

The DevExpress AI Chat (`DxAIChat`) component supports [OpenAI Assistants](https://techcommunity.microsoft.com/t5/ai-azure-ai-services-blog/announcing-azure-openai-service-assistants-public-preview/ba-p/4143217). This allows you to specify a model and supply supplementary documents (external knowledge). OpenAI parses these documents and searches through them to retrieve relevant content to answer user queries.

Add the following code to the _Program.cs_ file to register AI Assistant service in the application:

```cs
builder.Services.AddDevExpressAI((config) => {
    ...
    config.RegisterOpenAIAssistants(client, "gpt4o");
});
```

Include a supplementary document in the project file as an `EmbeddedResource`:

```
<EmbeddedResource Include="Data\Restaurant Menu.pdf" />
```

Handle the `Initialized` event and call the `UseAssistantAsync` method to supply a file to the Open AI Assistant. 

```razor
<DxAIChat CssClass="my-chat" Initialized="Initialized" />

@code {
    const string DocumentResourceName = "DevExpress.AI.Samples.Blazor.Data.Restaurant Menu.pdf";
    const string prompt = "...";

    async Task Initialized(IAIChat chat) {
        await chat.UseAssistantAsync(new OpenAIAssistantOptions(
            $"{Guid.NewGuid().ToString("N")}.pdf",
            Assembly.GetExecutingAssembly().GetManifestResourceStream(DocumentResourceName),
            prompt)
        );
    }
}
```

File to review: [Chat-Assistant.razor](./CS/DevExpress.AI.Samples.Blazor/Components/Pages/Chat-Assistant.razor)

### <a name="integration"></a> Integrate AI Chat into WinForms, WPF and .NET MAUI Apps

Thanks to both Blazor Hybrid technology and the BlazorWebView component, you can integrate DevExpress AI Chat (`DxAIChat`) into your next great WinForms, WPF, and .NET MAUI application.

Keys to implementation are as follows:

* The `ISelfEncapsulationService` interface allows you to work directly with the `DxAIChat` component instance/properties from your desktop or mobile app.
* Built-in `DxAIChat` wrappers initialize required Blazor Theme scripts.
* Custom CSS classes hide the built-in input field and the Send button (see _index.htm_).

Folders to review: [DevExpress.AI.Samples.MAUIBlazor](./CS/DevExpress.AI.Samples.MAUIBlazor/), [DevExpress.AI.Samples.WinBlazor](./CS/DevExpress.AI.Samples.WinBlazor/), [DevExpress.AI.Samples.WPFBlazor](./CS/DevExpress.AI.Samples.WPFBlazor/)

## Files to Review

* [Chat.razor](./CS/DevExpress.AI.Samples.Blazor/Components/Pages/Chat.razor)
* [Chat-CustomMessage.razor](./CS/DevExpress.AI.Samples.Blazor/Components/Pages/Chat-CustomMessage.razor)
* [Chat-CustomEmptyState.razor](./CS/DevExpress.AI.Samples.Blazor/Components/Pages/Chat-CustomEmptyState.razor)
* [Chat-MessageSent.razor](./CS/DevExpress.AI.Samples.Blazor/Components/Pages/Chat-MessageSent.razor)
* [Chat-Streaming.razor](./CS/DevExpress.AI.Samples.Blazor/Components/Pages/Chat-Streaming.razor)
* [Chat-Assistant.razor](./CS/DevExpress.AI.Samples.Blazor/Components/Pages/Chat-Assistant.razor)
* [Program.cs](./CS/DevExpress.AI.Samples.Blazor/Program.cs)

## Folders to Review

* [DevExpress.AI.Samples.MAUIBlazor](./CS/DevExpress.AI.Samples.MAUIBlazor/)
* [DevExpress.AI.Samples.WinBlazor](./CS/DevExpress.AI.Samples.WinBlazor/)
* [DevExpress.AI.Samples.WPFBlazor](./CS/DevExpress.AI.Samples.WPFBlazor/)

## Documentation

* [Create a Blazor Hybrid Project](https://docs.devexpress.com/Blazor/404118/get-started/create-project-hybrid)

## More Examples

* [Rich Text Editor and HTML Editor for Blazor - How to integrate AI-powered extensions](https://github.com/DevExpress-Examples/blazor-ai-integration-to-text-editors)


<!-- feedback -->
## Does this example address your development requirements/objectives?

[<img src="https://www.devexpress.com/support/examples/i/yes-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=devexpress-ai-chat-samples&~~~was_helpful=yes) [<img src="https://www.devexpress.com/support/examples/i/no-button.svg"/>](https://www.devexpress.com/support/examples/survey.xml?utm_source=github&utm_campaign=devexpress-ai-chat-samples&~~~was_helpful=no)

(you will be redirected to DevExpress.com to submit your response)
<!-- feedback end -->

