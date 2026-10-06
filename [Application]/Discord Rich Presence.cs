using DiscordRPC;
using LCLocalizationInterface.LimbusRegistry.PreviewCreator;

namespace LCLocalizationInterface
{
    public partial class App : Application
    {
        public static readonly DiscordRpcClient DiscordRPC = new(applicationID: "1350414892452286464");
        private static bool IsDiscordPresenceCleared = true;

        public static async void InitializeDiscordRPC()
        {
            App.DiscordRPC.Initialize();

            Timestamps SessionStartTimestamp = Timestamps.Now;

            while (true)
            {
                @Configurazione.JsonConfigurationFile.DiscordRPC_PROP.DiscordRPCScope_PROP CurrentScope = null!;

                if (@CurrentPreviewCreator.ActiveState == true)
                {
                    CurrentScope = @DataContextDomain.Configuration!.DiscordRPCOptions.PreviewCreator;
                }
                else if (@EditorModesShelf.CurrentEditorMode.Identifier == @EditorModesShelf.Types.EditorModeKey.MainMenu)
                {
                    CurrentScope = @DataContextDomain.Configuration!.DiscordRPCOptions.MainMenu;
                }
                else
                {
                    CurrentScope = @DataContextDomain.Configuration!.DiscordRPCOptions.ActiveEditor;
                }

                static string ApplyInsertions(string Value)
                {
                    Value = Value

                        .Replace("{selected_limbus_lang}", @DataContextDomain.Configuration!.PreviewSettings.CustomLang.AssociativeSettings.Selected)

                        .Replace("{ui_lang}", @DataContextDomain.Configuration!.Internal.UILanguage.RemovePrefix("[⇲] Assets Directory/※ Internal/Translation/"))
                        .Replace("{ui_theme}", @DataContextDomain.Configuration!.Internal.UITheme.RemovePrefix("[⇲] Assets Directory/※ Internal/Themes/"))

                        .Replace("{file_name}", @EditorModesShelf.CurrentEditorMode.CurrentFile?.Name)
                        .Replace("{editor_mode}", @EditorModesShelf.CurrentEditorMode.ReadableIdentifierName)

                        .Replace("{current_object_id}", @DataContextDomain.Editor.CurrentObjectID)
                        .Replace("{current_object_name}", @DataContextDomain.Editor.CurrentObjectName)

                        .Replace("{current_object_number}", @EditorModesShelf.CurrentEditorMode.VirtualCurrentObject is not null
                            ? $"{@EditorModesShelf.CurrentEditorMode.VirtualDataList.IndexOf(
                                @EditorModesShelf.CurrentEditorMode.Identifier is EditorModesShelf.Types.EditorModeKey.Skills
                                    ? @EditorModesShelf.Skills.CurrentSkill
                                    : @EditorModesShelf.CurrentEditorMode.VirtualCurrentObject
                                ) + 1}"
                            : "-1")
                        .Replace("{total_objects_count}", $"{@EditorModesShelf.CurrentEditorMode.VirtualDataList.Count}")


                        .Replace("{previewcreator_image_type}", $"{@DataContextDomain.PreviewCreator.ImageInfo.ImageType}")

                        .Replace("{previewcreator_portrait_type}", $"{@DataContextDomain.PreviewCreator.ImageInfo.Portrait.Type}")
                        .Replace("{previewcreator_rarity_or_risk_level}", $"{@DataContextDomain.PreviewCreator.ImageInfo.Header.RarityOrRiskLevel.Selected}")

                        .Replace("{previewcreator_sinner_name}", $"{@DataContextDomain.PreviewCreator.ImageInfo.Header.SinnerName.Text}")
                        .Replace("{previewcreator_identity_or_ego_name}", $"{@DataContextDomain.PreviewCreator.ImageInfo.Header.IdentityOrEGOName.Text.Replace("\\n", "")}")
                        
                        ;

                    if (Value.Length > 128)
                    {
                        Value = Value.Remove(128, 2) + "..";
                    }

                    return Value;
                }

                static string AssureStringLength(string Value, int MaxLength)
                {
                    if (Value.Length > MaxLength)
                    {
                        Value = Value[0..MaxLength];
                        Value = Value[..^2] + "..";
                    }

                    return Value;
                }
                static string AssureUrl(string Value)
                {
                    return Uri.TryCreate(Value, UriKind.Absolute, out _) ? Value : "";
                }
                
                List<DiscordRPC.Button> Buttons = [];
                void CheckAddButton(@Configurazione.JsonConfigurationFile.DiscordRPC_PROP.DiscordRPCScope_PROP.DiscordRPC_Button_PROP TargetButton)
                {
                    if (TargetButton.Enabled)
                    {
                        Buttons.Add(new() { Label = AssureStringLength(ApplyInsertions(TargetButton.Label), 31), Url = AssureUrl(AssureStringLength(TargetButton.URL, 512)) });
                    }
                }
                CheckAddButton(CurrentScope.Button1);
                CheckAddButton(CurrentScope.Button2);


                try
                {
                    if (App.DiscordRPC.IsDisposed == false && @DataContextDomain.Configuration!.DiscordRPCOptions.Enabled)
                    {
                        App.DiscordRPC.SetPresence(new RichPresence()
                        {
                            Timestamps = SessionStartTimestamp,

                            Type = @DataContextDomain.Configuration!.DiscordRPCOptions.ActivityType,

                            Details = AssureStringLength(ApplyInsertions(CurrentScope.Details), 128),
                            State = AssureStringLength(ApplyInsertions(CurrentScope.State), 128),

                            DetailsUrl = AssureUrl(AssureStringLength(CurrentScope.Details_URL, 256)),
                            StateUrl = AssureUrl(AssureStringLength(CurrentScope.State_URL, 256)),

                            Buttons = [.. Buttons]
                        });
                        IsDiscordPresenceCleared = false;
                    }
                    else if (IsDiscordPresenceCleared == false)
                    {
                        App.DiscordRPC.ClearPresence();
                        IsDiscordPresenceCleared = true;
                    }
                }
                catch (Exception Occurred)
                {
                    ErrorMessageWindow.ShowException(Occurred, $"This exception occured while trying to update Discord RPC state");
                }

                await Task.Delay(2000);
            }
        }
    }
}
