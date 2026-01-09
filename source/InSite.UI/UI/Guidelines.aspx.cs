using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;

using Humanizer;

using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI
{
    public partial class Guidelines : AdminBasePage
    {
        public string AroundHomeUrl => "https://ux.insite.com";

        public class GuidelineItem
        {
            public GuidelineItem(int sequence, string firstName, string lastName, string email, string phone, string rowClass)
            {
                Sequence = sequence;
                FirstName = firstName;
                LastName = lastName;
                Email = email;
                Phone = phone;
            }

            public int Sequence { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }

            public string RowClass { get; set; }
        }

        private ProgressExample _progressExample;

        protected string FFmpegFolderPath => ServiceLocator.AppSettings.Application.FFmpegFolderPath;

        public Guidelines()
        {
            _progressExample = new ProgressExample(this);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            EditButton.Click += (x, y) => ButtonOutput.Text = "Edit button clicked!";
            SaveButton.Click += (x, y) => ButtonOutput.Text = "Save button clicked!";
            DeleteButton.Click += (x, y) => ButtonOutput.Text = "Delete button clicked!";
            PrintButton.Click += (x, y) => ButtonOutput.Text = "Print button clicked!";

            UpdatePanelAjaxButton.Click += UpdatePanelButton_Click;
            UpdatePanelLongAjaxButton.Click += UpdatePanelLongAjaxButton_Click;
            UpdatePanelPostBackButton.Click += UpdatePanelButton_Click;

            _progressExample.OnInit();

            AudioInputBitrateTestSelector.AutoPostBack = true;
            AudioInputBitrateTestSelector.ValueChanged += (sender, args) =>
            {
                AudioInputBitrateTestInput.Bitrate = AudioInputBitrateTestSelector.Value.ToEnum<AudioBitrateMode>();
            };

            AudioInputBitrateTestInput.MediaCaptured += (sender, args) =>
            {
                var audio = AudioInputBitrateTestInput.AudioCapture;

                AudioInputBitrateTestStatus.AddMessage(
                    AlertType.Success,
                    "<p>Received media data information:</p><ul>" +
                    $"<li>Filename: {audio.Name}</li>" +
                    $"<li>File size: {audio.Size.Bytes().Humanize()}</li>" +
                    $"<li>Codec name: {audio.AudioStream.CodecLongName}</li>" +
                    $"<li>Sample rate: {audio.AudioStream.Audio.SampleRate:n0} Hz</li>" +
                    $"<li>Number of channels: {audio.AudioStream.Audio.ChannelsCount}</li>" +
                    $"<li>Channels layout: {audio.AudioStream.Audio.ChannelLayout}</li>" +
                    $"<li>Duration: {audio.Duration}</li>" +
                    $"<li>Bitrate: {audio.Bitrate:n0} b/s</li>" +
                    "</ul>");

                audio.Delete();
            };
            AudioInputBitrateTestInput.MediaCaptureFailed += (sender, args) =>
            {
                AudioInputBitrateTestStatus.AddMessage(AlertType.Error, "<strong>FAILED</strong> " + args.Value);
                AudioInputBitrateTestInput.AudioCapture?.Delete();
            };
        }

        private void UpdatePanelLongAjaxButton_Click(object sender, EventArgs e)
        {
            UpdatePanelButton_Click(sender, e);

            Thread.Sleep(3000);
        }

        private void UpdatePanelButton_Click(object sender, EventArgs e)
        {
            UpdatePanelOutput.Text = UpdatePanelInput.Text;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            BindModelToControls();

            TableExample.DataSource = CreateDataSource();
            TableExample.DataBind();

            DateSelector.Value = DateTime.Parse("2021-09-15");
            DateTimeOffsetSelector.Value = DateTimeOffset.Parse("2021-09-15 2:42:10 PM -06:00");

            FindAchievement1.Filter.OrganizationIdentifiers.Add(Organization.Identifier);
            FindAchievement2.Filter.OrganizationIdentifiers.Add(Organization.Identifier);
            FindAchievement3.Filter.OrganizationIdentifiers.Add(Organization.Identifier);

            FindDepartment1.OrganizationIdentifier = Organization.Identifier;
            FindDepartment2.OrganizationIdentifier = Organization.Identifier;
            FindDepartment3.OrganizationIdentifier = Organization.Identifier;

            FindGroup1.Filter.OrganizationIdentifier = Organization.Identifier;
            FindGroup1.Filter.GroupType = GroupTypes.Employer;
            FindGroup2.Filter.OrganizationIdentifier = Organization.Identifier;
            FindGroup2.Filter.GroupType = GroupTypes.Employer;
            FindGroup3.Filter.OrganizationIdentifier = Organization.Identifier;
            FindGroup3.Filter.GroupType = GroupTypes.Employer;

            FindSurvey1.Filter.OrganizationIdentifier = Organization.Identifier;
            FindSurvey2.Filter.OrganizationIdentifier = Organization.Identifier;
            FindSurvey3.Filter.OrganizationIdentifier = Organization.Identifier;

            FindUser1.Filter.OrganizationIdentifiers = new Guid[] { Organization.Identifier };
            FindUser2.Filter.OrganizationIdentifiers = new Guid[] { Organization.Identifier };
            FindUser3.Filter.OrganizationIdentifiers = new Guid[] { Organization.Identifier };

            FindBank1.Filter.OrganizationIdentifiers = new Guid[] { Organization.Identifier };
            FindBank2.Filter.OrganizationIdentifiers = new Guid[] { Organization.Identifier };
            FindBank3.Filter.OrganizationIdentifiers = new Guid[] { Organization.Identifier };

            FindBankForm1.Filter.OrganizationIdentifiers = new Guid[] { Organization.Identifier };
            FindBankForm2.Filter.OrganizationIdentifiers = new Guid[] { Organization.Identifier };
            FindBankForm3.Filter.OrganizationIdentifiers = new Guid[] { Organization.Identifier };

            FindEvent1.Filter.OrganizationIdentifier = Organization.Identifier;
            FindEvent2.Filter.OrganizationIdentifier = Organization.Identifier;
            FindEvent3.Filter.OrganizationIdentifier = Organization.Identifier;

            FindPerson1.Filter.OrganizationIdentifier = Organization.Identifier;
            FindPerson2.Filter.OrganizationIdentifier = Organization.Identifier;
            FindPerson3.Filter.OrganizationIdentifier = Organization.Identifier;

            FindPeriod1.Filter.OrganizationIdentifier = Organization.Identifier;
            FindPeriod2.Filter.OrganizationIdentifier = Organization.Identifier;
            FindPeriod3.Filter.OrganizationIdentifier = Organization.Identifier;

            {
                MarkdownUpload.FolderPath = $"/guidline/2932f4c3-676f-442f-b65a-ede8e369310c";

                var text = new Shift.Common.MultilingualString();
                text["en"] = "This is **Markdown** content.";
                text["fr"] = "Ceci est du contenu **Markdown**.";

                MarkdownTranslation.Text = text;
            }

            {
                HtmlUpload.FolderPath = $"/guidline/e4b5b009-13a9-446d-b28c-70f3264a9208";

                var text = new Shift.Common.MultilingualString();
                text["en"] = "This is <b>HTML</b> content.";
                text["fr"] = "Il s'agit de contenu <b>HTML</b>.";

                HtmlTranslation.Text = text;
            }

            UpdatePanelInput.Text = DateTime.Now.ToString();

            _progressExample.OnLoad();

            AudioInputBitrateTestSelector.LoadItems(
                AudioBitrateMode.kb_8,
                AudioBitrateMode.kb_16,
                AudioBitrateMode.kb_32,
                AudioBitrateMode.kb_64,
                AudioBitrateMode.kb_128,
                AudioBitrateMode.kb_256);
            AudioInputBitrateTestSelector.Value = AudioInputBitrateTestInput.Bitrate.GetName();
        }

        protected void BindModelToControls()
        {
            PageHelper.AutoBindHeader(this);
            PageHelper.HideSideContent(this);
        }

        private List<GuidelineItem> CreateDataSource()
        {
            return new List<GuidelineItem>
            {
                new GuidelineItem(1, "Alice", "O'Wonderland", "alice@example.com", "123 456 7890", null),
                new GuidelineItem(2, "Boba", "Fett", "boba@example.com", "123 456 7890", null),
                new GuidelineItem(3, "Charlie", "Brown", "charlie@example.com.com", "123 456 7890", "table-success")
            };
        }

        private class ProgressExample
        {
            #region Classes

            private class File
            {
                public static File[] Array { get; } = new[]
                {
                    new File("Database.bak", 1.28),
                    new File("Archive.zip", 0.82),
                    new File("BinaryFile.bin", 0.9),
                };

                #region Properties

                public string FileName { get; }
                public long FileSize { get; }
                public string FileSizeString => FileSize.Bytes().Humanize("GB");

                #endregion

                #region Construction

                public File(string name, double sizeGb)
                {
                    FileName = name;
                    FileSize = GygabytesToBytes(sizeGb);
                }

                #endregion
            }

            private class FileReader
            {
                #region Constants

                private static readonly int UpdateInterval = 250;

                private static readonly int MinReadSizePerSec = (int)MegabytesToBytes(50);
                private static readonly int MaxReadSizePerSec = (int)MegabytesToBytes(140);

                private static readonly int MinReadSizePerInterval = (int)((decimal)MinReadSizePerSec / 1000 * UpdateInterval);
                private static readonly int MaxReadSizePerInterval = (int)((decimal)MaxReadSizePerSec / 1000 * UpdateInterval);

                #endregion

                #region Properties

                public bool CanRead => _position < _file.FileSize;

                #endregion

                #region Fields

                private File _file;
                private long _position;

                #endregion

                #region Construction

                public FileReader(File file)
                {
                    _file = file;
                    _position = 0L;
                }

                #endregion

                #region Methods

                public long ReadNext()
                {
                    var delay = UpdateInterval;
                    var bytesRead = (long)RandomNumberGenerator.Instance.Next(MinReadSizePerInterval, MaxReadSizePerInterval);

                    if (_position + bytesRead > _file.FileSize)
                    {
                        bytesRead = _file.FileSize - _position;

                        if (bytesRead < MinReadSizePerInterval)
                            delay = (int)(delay * ((double)bytesRead / MinReadSizePerInterval));
                    }

                    _position += bytesRead;

                    Thread.Sleep(delay);

                    return bytesRead;
                }

                #endregion
            }

            private class ProgressData
            {
                public string FileName { get; set; }

                public DateTime StartedOn { get; set; }

                public ProgressBarData<int> FileCount { get; } = new ProgressBarData<int>();

                public ProgressBarData<long> FileSize { get; } = new ProgressBarData<long>();

                public ProgressBarData<long> UploadSize { get; } = new ProgressBarData<long>();
            }

            private class ProgressBarData<TValue>
            {
                public TValue Value { get; set; }

                public TValue Total { get; set; }
            }

            #endregion

            #region Fields

            private Guidelines Page;

            #endregion

            #region Initialization and Loading

            public ProgressExample(Guidelines page)
            {
                Page = page;
            }

            public void OnInit()
            {
                Page.ProgressPanelTest.RequestCancelled += ProgressPanelTest_RequestCancelled;
                Page.ProgressPanelAjaxTestButton.Click += StartActionButton_Click;
                Page.ProgressPanelPostBackTestButton.Click += StartActionButton_Click;
            }

            public void OnLoad()
            {
                if (Page.IsPostBack)
                    return;

                Page.ProgressPanelTestRepeater.DataSource = File.Array;
                Page.ProgressPanelTestRepeater.DataBind();

                ShowCancelMessage();
            }

            #endregion

            #region Event handlers

            private void ProgressPanelTest_RequestCancelled(object sender, EventArgs e)
            {
                var message = GetCancelMessage(Page.ProgressPanelTest.ContextID);
                if (message.IsNotEmpty())
                    ShowMessage("bg-danger text-white", message);
            }

            private void StartActionButton_Click(object sender, EventArgs e)
            {
                var data = new ProgressData();
                {
                    data.StartedOn = DateTime.UtcNow;

                    data.FileCount.Total = File.Array.Length;
                    data.FileSize.Total = File.Array.Sum(x => x.FileSize);
                }

                try
                {
                    foreach (var file in File.Array)
                    {
                        var reader = new FileReader(file);

                        data.FileName = file.FileName;
                        data.UploadSize.Value = 0;
                        data.UploadSize.Total = file.FileSize;

                        UpdateProgress(data);

                        while (reader.CanRead)
                        {
                            if (IsCancelled())
                            {
                                SetCancelMessage("Cancelled by user! The process stopped on " + data.FileName);
                                return;
                            }

                            var bytesCount = reader.ReadNext();

                            data.FileSize.Value += bytesCount;
                            data.UploadSize.Value += bytesCount;

                            UpdateProgress(data);
                        }

                        data.FileCount.Value += 1;
                    }

                    UpdateProgress(data);

                    ShowMessage("bg-info text-white", "Files uploaded successfully!");
                }
                finally
                {
                    Page.ProgressPanelTest.RemoveContext();
                }
            }

            #endregion

            #region Methods

            private void SetCancelMessage(string message)
            {
                Page.Session[Page.ProgressPanelTest.ContextID] = message;
            }

            private string GetCancelMessage(string contextId)
            {
                var message = Page.Session[contextId] as string;

                Page.Session.Remove(contextId);

                return message;
            }

            private void ShowCancelMessage()
            {
                var pContextId = Page.Request.QueryString["progressContext"];
                if (pContextId.IsEmpty())
                    return;

                var message = GetCancelMessage(pContextId);
                if (message.IsNotEmpty())
                    ScriptManager.RegisterStartupScript(
                        Page,
                        typeof(Guidelines),
                        "show_progress_message",
                        $"alert({HttpUtility.JavaScriptStringEncode(message, true)})",
                        true
                    );
            }

            private void ShowMessage(string css, string message)
            {
                var literal = Page.ProgressPanelTestStatus;

                literal.Text =
                    $"<div id='{literal.ClientID}' class=\"p-3 mt-3 mb-2 {css}\">" +
                    $"<strong>{message}</strong>" +
                    $"</div>";

                ScriptManager.RegisterStartupScript(
                    Page,
                    typeof(Guidelines),
                    "hide_progress_message",
                    $"setTimeout(function () {{ $('#{literal.ClientID}').remove(); }}, 5000)",
                    true
                );
            }

            private bool IsCancelled() => !Page.Response.IsClientConnected;

            private void UpdateProgress(ProgressData data)
            {
                Page.ProgressPanelTest.UpdateContext(context =>
                {
                    var timeElapsed = DateTime.UtcNow - data.StartedOn;
                    var fileSizeRatio = (decimal)data.FileSize.Value / data.FileSize.Total;
                    var uploadSizeRatio = (decimal)data.UploadSize.Value / data.UploadSize.Total;

                    var fileCountBar = (ProgressIndicator.ContextData)context.Items["ProgressFileCount"];
                    fileCountBar.Total = data.FileCount.Total;
                    fileCountBar.Value = data.FileCount.Value;

                    var fileSizeBar = (ProgressIndicator.ContextData)context.Items["ProgressFileSize"];
                    fileSizeBar.Total = 1000;
                    fileSizeBar.Value = (int)(fileSizeRatio * fileSizeBar.Total);

                    var uploadBar = (ProgressIndicator.ContextData)context.Items["ProgressUpload"];
                    uploadBar.Total = 1000;
                    uploadBar.Value = (int)(uploadSizeRatio * uploadBar.Total);

                    context.Variables["filename"] = data.FileName;
                    context.Variables["total_mb"] = data.FileSize.Value.Bytes().Humanize("MB");
                    context.Variables["total_gb"] = data.FileSize.Value.Bytes().Humanize("GB");

                    if (timeElapsed.TotalSeconds > 1)
                    {
                        context.Variables["upload_speed"] =
                            (data.FileSize.Value / timeElapsed.TotalSeconds).Bytes().Humanize("MB") + "/s";

                        context.Variables["time_remaining"] = string.Format(
                            "{0:hh}:{0:mm}:{0:ss}s",
                            Clock.TimeRemaining(data.FileSize.Total, data.FileSize.Value, data.StartedOn)
                        );

                        context.Variables["time_estimated"] = string.Format(
                            "{0:hh}:{0:mm}:{0:ss}s",
                            Clock.TimeEstimated(data.FileSize.Total, data.FileSize.Value, data.StartedOn)
                        );
                    }
                    else
                    {
                        context.Variables["upload_speed"] = "N/A";
                        context.Variables["time_remaining"] = "N/A";
                        context.Variables["time_estimated"] = "N/A";
                    }
                });
            }

            #endregion

            #region Helpers

            private static long GygabytesToBytes(double valueGb) => (long)(valueGb * 1024 * 1024 * 1024);

            private static long MegabytesToBytes(double valueMb) => (long)(valueMb * 1024 * 1024);

            #endregion
        }
    }
}