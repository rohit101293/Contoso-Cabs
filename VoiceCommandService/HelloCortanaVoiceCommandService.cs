using System;
using ContosoCabs.ResponseModels.Private;
using ContosoCabs.Service;
using ContosoCabs.ServiceModels;
using ContosoCabs.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;
using Windows.ApplicationModel.Resources.Core;
using Windows.ApplicationModel.AppService;
using Windows.Storage;



namespace ContosoCabs.VoiceCommands
{
    public sealed class HelloCortanaVoiceCommandService : IBackgroundTask
    {
        private BackgroundTaskDeferral serviceDeferral;
        VoiceCommandServiceConnection voiceServiceConnection;
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            //service deferral so that the  app service is not terminated while handling the voice command.
            this.serviceDeferral = taskInstance.GetDeferral();

            taskInstance.Canceled += OnTaskCanceled;

            var triggerDetails =
              taskInstance.TriggerDetails as AppServiceTriggerDetails;

            if (triggerDetails != null && triggerDetails.Name == "HelloCortanaVoiceCommandService")
            {
                try
                {
                    voiceServiceConnection =
                      VoiceCommandServiceConnection.FromAppServiceTriggerDetails(
                        triggerDetails);

                    voiceServiceConnection.VoiceCommandCompleted +=
                      VoiceCommandCompleted;

                    VoiceCommand voiceCommand = await
                    voiceServiceConnection.GetVoiceCommandAsync();

                    switch (voiceCommand.CommandName)
                    {
                        case "showInCanvas":
                            {
                                var destination =
                                  voiceCommand.Properties["destination"][0];
                                SendCompletionMessageForDestination(destination);
                                break;
                            }

                        
                        default:
                            LaunchAppInForeground();
                            break;
                    }
                }
                finally
                {
                    if (this.serviceDeferral != null)
                    {
                        // Complete the service deferral.
                        this.serviceDeferral.Complete();
                    }
                }
            }
        }

 private void VoiceCommandCompleted(VoiceCommandServiceConnection sender, VoiceCommandCompletedEventArgs args)
        {
            if (this.serviceDeferral != null)
            {
                //service deferral so that the  app service is not terminated while handling the voice command.
                this.serviceDeferral.Complete();
            }
        }
        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            System.Diagnostics.Debug.WriteLine("Task cancelled, clean up");
            if (this.serviceDeferral != null)
            {
                //Complete the service deferral
                this.serviceDeferral.Complete();
            }
        }
        private async void SendCompletionMessageForDestination(string destination)
        {
            
            var userMessage = new VoiceCommandUserMessage();
            userMessage.DisplayMessage = "Here’s your cab details.";
            userMessage.SpokenMessage = "Ola cab /Uber Cab.";

           
            var destinationsContentTiles = new List<VoiceCommandContentTile>();

            var destinationTile = new VoiceCommandContentTile();
            destinationTile.ContentTileType =
              VoiceCommandContentTileType.TitleWith68x68IconAndText;
            destinationTile.Image = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///ContosoCabs.VoiceCommands/Images/cablogo.png"));
            destinationTile.AppLaunchArgument = destination;
            destinationTile.Title = "Hyderabad";
            destinationTile.TextLine1 = "you have been amazing";
            destinationsContentTiles.Add(destinationTile);

            // Create the VoiceCommandResponse from the userMessage and list    
            // of content tiles.
            var response =
              VoiceCommandResponse.CreateResponse(userMessage, destinationsContentTiles);
            response.AppLaunchArgument =
              string.Format("destination={0}”, “Hyderabad");
            await voiceServiceConnection.ReportSuccessAsync(response);
        }

        private async void LaunchAppInForeground()
        {
            var userMessage = new VoiceCommandUserMessage();
            userMessage.SpokenMessage = "Launching Contoso Cabs";

            var response = VoiceCommandResponse.CreateResponse(userMessage);
                response.AppLaunchArgument = "";

            await voiceServiceConnection.RequestAppLaunchAsync(response);
        }
    }
}
       