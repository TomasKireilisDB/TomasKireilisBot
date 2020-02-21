using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TomasKireilisBot.DataModels;
using TomasKireilisBot.Helpers;
using TomasKireilisBot.Services.BitbucketService;
using TomasKireilisBot.Services.Timer;

namespace TomasKireilisBot.Dialogs
{
    public class ApprovePullRequestDialog : CancelAndHelpDialog
    {
        private readonly Timers _timers;
        private readonly IInnerBitbucketClient _innerBitbucketClient;

        public ApprovePullRequestDialog(IInnerBitbucketClient innerBitbucketClient, Timers timers) : base(nameof(ApprovePullRequestDialog))
        {
            _innerBitbucketClient = innerBitbucketClient;
            _timers = timers;
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                FinalStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            try
            {
                var dataList = new List<string>();
                var activityValue = "";
                try
                {
                    activityValue = ((BitBucketConversationVariables)stepContext.Options).Data;
                }
                catch
                {
                    return await stepContext.EndDialogAsync(null, cancellationToken);
                }

                if (string.IsNullOrEmpty(activityValue))
                {
                    return await stepContext.EndDialogAsync(null, cancellationToken);
                }

                dataList = activityValue.Split('>', StringSplitOptions.RemoveEmptyEntries).ToList();

                if (dataList.Count == 5 && dataList[0]?.ToLower() == "approvepullrequest")
                {
                    await stepContext.Context.SendActivityAsync("CheckingPullRequestStatus...", cancellationToken: cancellationToken);
                    bool resultFound = false;

                    foreach (
                        var globalVariable
                        in (await GlobalVariablesService.GetBitBucketConversationVariables(stepContext.Context.Activity.Recipient.Id)).GlobalVariables
                        )

                    {
                        foreach (var project in globalVariable.Projects)
                        {
                            foreach (var repositoryName in project.RepositoryNames)
                            {
                                if (string.Equals(globalVariable.BaseUrl, dataList[1], StringComparison.CurrentCultureIgnoreCase) &&
                                    string.Equals(project.ProjectName, dataList[2], StringComparison.CurrentCultureIgnoreCase) &&
                                    string.Equals(repositoryName, dataList[3], StringComparison.CurrentCultureIgnoreCase))
                                {
                                    resultFound = true;
                                    try
                                    {
                                        await _innerBitbucketClient.ApprovePullRequest(globalVariable.BaseUrl,
                                            project.ProjectName,
                                            repositoryName,
                                            globalVariable.PersonalAccessToken,
                                            globalVariable.Password,
                                            globalVariable.UserName, long.Parse(dataList[4]));
                                        await stepContext.Context.SendActivityAsync("Approved successfully", cancellationToken: cancellationToken);
                                    }
                                    catch (Exception e)
                                    {
                                        await stepContext.Context.SendActivityAsync("Oooops. Something went wrong. Could not approve pull request",
                                            cancellationToken: cancellationToken);
                                        await stepContext.Context.SendActivityAsync(e.Message,
                                            cancellationToken: cancellationToken);
                                    }

                                    break;
                                }
                            }
                        }
                    }

                    if (!resultFound)
                    {
                        await stepContext.Context.SendActivityAsync("Could not find such repository. Check project and repository names",
                            cancellationToken: cancellationToken);
                    }
                    return await stepContext.EndDialogAsync(null, cancellationToken);
                }
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }
            catch (Exception e)
            {
                await stepContext.Context.SendActivityAsync(e.Message,
                           cancellationToken: cancellationToken);
            }
        }
    }
}