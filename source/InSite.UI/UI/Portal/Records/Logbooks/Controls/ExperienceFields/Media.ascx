<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Media.ascx.cs" Inherits="InSite.UI.Portal.Records.Logbooks.Controls.ExperienceFields.Media" %>

<div class="form-group mb-3 position-relative">
    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />
    <insite:UpdatePanel runat="server" ID="UpdatePanel">
        <ContentTemplate>
            <label class="form-label">
                <asp:Literal runat="server" ID="FieldTitle" />
                <insite:CustomValidator runat="server" ID="FieldValidator" />
            </label>

            <div class="mb-2">
                <insite:TextBox runat="server" ID="MediaName" EmptyMessage="Evidence Name" MaxLength="100" />
            </div>

            <div class="mb-2">
                <insite:ComboBox runat="server" ID="MediaType">
                    <Items>
                        <insite:ComboBoxOption Text="Video" Value="video" Selected="true" />
                        <insite:ComboBoxOption Text="Audio" Value="audio" />
                    </Items>
                </insite:ComboBox>
            </div>

            <div class="mb-2">
                <insite:InputVideo runat="server" ID="MediaVideoInput" UploadMode="PostBack" Visible="false" Hidden="true" />
                <insite:OutputVideo runat="server" ID="MediaVideoOutput" AllowDelete="true" Visible="false" Hidden="true" />

                <insite:InputAudio runat="server" ID="MediaAudioInput" UploadMode="PostBack" Visible="false" Hidden="true" />
                <insite:OutputAudio runat="server" ID="MediaAudioOutput" AllowDelete="true" Visible="false" Hidden="true" />
            </div>

            <div class="form-text">
                <asp:Literal runat="server" ID="HelpText" />
            </div>

            <asp:HiddenField runat="server" ID="FileUrl" />
        </ContentTemplate>
    </insite:UpdatePanel>
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            const instance = window.<%= UniqueID %> = {};

            let input;
            let videoInput, videoOutput;
            let audioInput, audioOutput;
            let previousInput, previousOutput;

            Sys.Application.add_load(function () {
                input = document.getElementById('<%= FileUrl.ClientID %>');
                videoInput = document.getElementById('<%= MediaVideoInput.ClientID %>');
                videoOutput = document.getElementById('<%= MediaVideoOutput.ClientID %>');
                audioInput = document.getElementById('<%= MediaAudioInput.ClientID %>');
                audioOutput = document.getElementById('<%= MediaAudioOutput.ClientID %>');

                if (videoInput && videoOutput)
                    initVideo();
                else if (audioInput && audioOutput)
                    initAudio();
            });

            // video

            function initVideo() {
                const hasValue = !!input.value;

                if (videoInput != previousInput) {
                    videoInput.addEventListener(inSite.common.inputVideo.event.uploaded, onVideoUploaded);

                    if (!hasValue)
                        videoInput.inputVideo.show();
                    else
                        videoInput.inputVideo.hide();

                    previousInput = videoInput;
                }

                if (videoOutput != previousOutput) {
                    videoOutput.addEventListener(inSite.common.outputVideo.event.delete, onVideoDeleted);

                    if (hasValue) {
                        videoOutput.outputVideo.show();
                        videoOutput.outputVideo.setData(input.value);
                    } else {
                        videoOutput.outputVideo.hide();
                    }

                    previousOutput = videoOutput;
                }
            }

            function onVideoUploaded() {
                videoInput.inputVideo.hide();

                videoOutput.outputVideo.setData(videoInput.inputVideo.mediaData);
                videoOutput.outputVideo.show();
            }

            function onVideoDeleted(e) {
                e.preventDefault();

                videoOutput.outputVideo.stop();

                if (!confirm('Are you sure you want to delete this video?'))
                    return;

                input.value = '';

                videoOutput.outputVideo.setData(null);
                videoOutput.outputVideo.hide();

                videoInput.inputVideo.clear();
                videoInput.inputVideo.show();
            }

            // audio

            function initAudio() {
                const hasValue = !!input.value;

                if (audioInput != previousInput) {
                    audioInput.addEventListener(inSite.common.inputAudio.event.uploaded, onAudioUploaded);

                    if (!hasValue)
                        audioInput.inputAudio.show();
                    else 
                        audioInput.inputAudio.hide();

                    previousInput = audioInput;
                }

                if (audioOutput != previousOutput) {
                    audioOutput.addEventListener(inSite.common.outputAudio.event.delete, onAudioDeleted);

                    if (hasValue) {
                        audioOutput.outputAudio.show();
                        audioOutput.outputAudio.setData(input.value);
                    } else {
                        audioOutput.outputAudio.hide();
                    }

                    previousOutput = audioOutput;
                }
            }

            function onAudioUploaded() {
                audioInput.inputAudio.hide();

                audioOutput.outputAudio.setData(audioInput.inputAudio.mediaData);
                audioOutput.outputAudio.show();
            }

            function onAudioDeleted(e) {
                e.preventDefault();

                audioOutput.outputAudio.stop();

                if (!confirm('Are you sure you want to delete this audio?'))
                    return;

                input.value = '';

                audioOutput.outputAudio.setData(null);
                audioOutput.outputAudio.hide();

                audioInput.inputAudio.clear();
                audioInput.inputAudio.show();
            }
        })();
    </script>
</insite:PageFooterContent>
