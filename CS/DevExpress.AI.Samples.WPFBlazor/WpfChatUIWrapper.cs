using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using DevExpress.Blazor;
using DevExpress.AIIntegration.Blazor.Chat;

namespace DevExpress.AI.Samples.WPFBlazor
{
    public interface ISelfEncapsulationService
    {
        void Initialize(WpfChatUIWrapper dxChatUI);
    }

    class DxChatEncapsulationService : ISelfEncapsulationService
    {
        public WpfChatUIWrapper? DxChatUI { get; set; }
        public void Initialize(WpfChatUIWrapper dxChatUI)
        {
            this.DxChatUI = dxChatUI;
        }
    }

    public class WpfChatUIWrapper: DxAIChat
    {
        [Inject] ISelfEncapsulationService SelfIncapsulationService { get; set; } = default!;
        protected override void OnInitialized()
        {
            SelfIncapsulationService.Initialize(this);
            base.OnInitialized();
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            DxResourceManager.RegisterScripts()(builder);
            base.BuildRenderTree(builder);
        }
    }

    public class MyDictionary : Dictionary<string, object>
    {
    }
}