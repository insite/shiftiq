<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Guidelines.aspx.cs" Inherits="InSite.UI.Guidelines" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Nav runat="server">

        <insite:NavItem runat="server" Title="Accordion">

            <h2>Accordions</h2>

            <pre><code>
&lt;insite:Accordion runat="server"&gt;
    &lt;insite:AccordionPanel runat="server" Icon="fas fa-abacus" Title="Counters"&gt;
    &lt;/insite:AccordionPanel&gt;
    &lt;insite:AccordionPanel runat="server" Icon="fas fa-tools" Title="Administration Tools"&gt;
    &lt;/insite:AccordionPanel&gt;
&lt;/insite:Accordion&gt;
                </code></pre>

            <h3 class="mt-3">Examples</h3>

            <insite:Accordion runat="server">
                <insite:AccordionPanel runat="server" Icon="fas fa-abacus" Title="Counters">
                    Counters Panel Content
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="fas fa-tools" Title="Administration Tools">
                    Administration Tools Panel Content
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="fas fa-history" Title="Recent Changes">
                    Recent Changes Panel Content
                </insite:AccordionPanel>
            </insite:Accordion>

        </insite:NavItem>

        <insite:NavItem runat="server" Title="Alert">

            <h2>Alerts</h2>

            <p>
                <a href="<%= AroundHomeUrl %>/components/alerts.html">Alerts</a> should always include an icon.
            </p>

            <pre><code>
&lt;insite:Alert runat="server" Icon="far fa-info-square fs-xl" Text="This is an information alert." Indicator="Information" /&gt;
&lt;insite:Alert runat="server" Icon="far fa-check-circle fs-xl" Text="Here is an example of an alert displayed after some successful operation." Indicator="Success" /&gt;
                </code></pre>

            <h3 class="mt-3">Examples</h3>

            <insite:Alert runat="server" Icon="far fa-info-square fs-xl" Text="Information alert." Indicator="Information" />
            <insite:Alert runat="server" Icon="far fa-check-circle fs-xl" Text="Success alert." Indicator="Success" />
            <insite:Alert runat="server" Icon="far fa-stop-circle fs-xl" Text="Error alert." Indicator="Error" />
            <insite:Alert runat="server" Icon="far fa-exclamation-triangle fs-xl" Text="Warning alert." Indicator="Warning" />

            <insite:Alert runat="server" Text="Default alert." />

        </insite:NavItem>

        <insite:NavItem runat="server" Title="AJAX">
            <div class="row">
                <div class="col-lg-9 content py-4">

                    <section class="pb-5">

                        <h3>UpdatePanel and UpdateProgress</h3>

                        <p>
                            Use UpdatePanel when you need to update only part of a page during postback instead of reloading the entire page.
                        If you assume that an AJAX request can take a noticeable amount of time then you should use UpdateProgress to show the users that the page doing request at the moment.
                        </p>

                        <pre><code>
    &lt;insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" /&gt;

    &lt;insite:UpdatePanel runat="server" ID="UpdatePanel"&gt;
        &lt;ContentTemplate&gt;

        &lt;/ContentTemplate&gt;
    &lt;/insite:UpdatePanel&gt;
                    </code></pre>

                        <div class="card mt-4 mb-4">
                            <div class="card-header">Example</div>
                            <div class="card-body">
                                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanelExample" />

                                <insite:UpdatePanel runat="server" ID="UpdatePanelExample">
                                    <Triggers>
                                        <asp:PostBackTrigger ControlID="UpdatePanelPostBackButton" />
                                    </Triggers>
                                    <ContentTemplate>
                                        <strong>Input:</strong>
                                        <insite:TextBox runat="server" ID="UpdatePanelInput" />

                                        <strong>Output:</strong>
                                        <insite:TextBox runat="server" ID="UpdatePanelOutput" />

                                        <div class="mt-4">
                                            <insite:Button runat="server" ID="UpdatePanelAjaxButton" Icon="fas fa-upload" Text="AJAX Request" ButtonStyle="Success" />
                                            <insite:Button runat="server" ID="UpdatePanelLongAjaxButton" Icon="fas fa-upload" Text="Long AJAX Request" ButtonStyle="Warning" />
                                            <insite:Button runat="server" ID="UpdatePanelPostBackButton" Icon="fas fa-upload" Text="Regular Request" ButtonStyle="Danger" />
                                        </div>
                                    </ContentTemplate>
                                </insite:UpdatePanel>
                            </div>
                        </div>

                        <h3>LoadingPanel</h3>

                        <p>
                            Use LoadingPanel when you need to show that the page is busy but you don't use UpdatePanel/UpdateProgress.
                        This is handy a component when you initiate an AJAX request using jQuery.
                        </p>

                        <pre><code>
    &lt;insite:LoadingPanel runat="server" ID="LoadingPanel" /&gt;

    &lt;script type="text/javascript"&gt;
        $('#LoadingPanel').show();
    &lt;/script&gt;
                    </code></pre>

                        <div class="card mt-4 mb-4">
                            <div class="card-header">Example</div>
                            <div class="card-body">
                                <br />
                                Some content here...<br />
                                <br />
                                <insite:LoadingPanel runat="server" ID="LoadingPanelExample" />
                            </div>
                            <div class="card-footer">
                                <insite:Button runat="server" ID="ToggleLoadingPanelExample" ButtonStyle="Default"
                                    Icon="fas fa-eye" Text="Show/Hide Loading Panel" />
                                <script type="text/javascript">
                                    (function () {
                                        $('#<%= ToggleLoadingPanelExample.ClientID %>').on('click', function (e) {
                                            e.preventDefault();

                                            $('#<%= LoadingPanelExample.ClientID %>').toggle();
                                        });
                                    })();
                                </script>
                            </div>
                        </div>


                        <h3>ProgressPanel</h3>

                        <p>
                            Use ProgressPanel when you need to display the progress of a long-running process triggered by PostBack or AJAX request.
                        </p>

                        <pre><code>
    &lt;insite:ProgressPanel runat="server" ID="ProgressPanel" HeaderText="Upload Files" Cancel="Redirect"&gt;
        &lt;Triggers&gt;
            &lt;insite:ProgressControlTrigger ControlID="UploadButton" /&gt;
        &lt;/Triggers&gt;
        &lt;Items&gt;
            &lt;insite:ProgressIndicator ID="ProgressBar" Label="{percent}%" /&gt;
        &lt;/Items&gt;
    &lt;/insite:ProgressPanel&gt;

                    </code></pre>

                        <insite:UpdatePanel runat="server">
                            <Triggers>
                                <asp:PostBackTrigger ControlID="ProgressPanelPostBackTestButton" />
                            </Triggers>
                            <ContentTemplate>

                                <div class="card mt-4 mb-4">
                                    <div class="card-header">Example</div>
                                    <div class="card-body">
                                        <strong>Upload Files:</strong>
                                        <asp:Repeater runat="server" ID="ProgressPanelTestRepeater">
                                            <HeaderTemplate>
                                                <ul>
                                            </HeaderTemplate>
                                            <FooterTemplate></ul></FooterTemplate>
                                            <ItemTemplate>
                                                <li><%# Eval("FileName") %> - (<%# Eval("FileSizeString") %>)</li>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                    <div class="card-footer">
                                        <insite:Button runat="server" ID="ProgressPanelAjaxTestButton" Icon="fas fa-upload" Text="AJAX Upload" ButtonStyle="Success" />
                                        <insite:Button runat="server" ID="ProgressPanelPostBackTestButton" Icon="fas fa-upload" Text="PostBack Upload" ButtonStyle="Warning" />
                                        <asp:Literal runat="server" ID="ProgressPanelTestStatus" />
                                    </div>
                                </div>

                            </ContentTemplate>
                        </insite:UpdatePanel>

                        <insite:ProgressPanel runat="server" ID="ProgressPanelTest" HeaderText="Upload Files" Cancel="Redirect">
                            <ClientEvents OnPollingStart="progressTest.onPollStart"
                                OnPollingError="progressTest.onPollError"
                                OnPollingStopped="progressTest.onPollStopped"
                                OnCancelled="progressTest.onCancelled"
                                OnSubmitDetected="progressTest.onSubmitStart"
                                OnSubmitError="progressTest.onSubmitError" />
                            <Triggers>
                                <insite:ProgressControlTrigger ControlID="ProgressPanelAjaxTestButton" />
                                <insite:ProgressControlTrigger ControlID="ProgressPanelPostBackTestButton" />
                            </Triggers>
                            <Items>
                                <insite:ProgressIndicator Name="ProgressFileCount" Label="{value}/{total}" />
                                <insite:ProgressIndicator Name="ProgressFileSize" Label="{total_gb}" />
                                <insite:ProgressIndicator Name="ProgressUpload" Caption="File: {filename}{running_ellipsis}" Label="{percent}%" />
                                <insite:ProgressStatus Text="Elapsed time: {time_elapsed}s" />
                                <insite:ProgressStatus>
                                Remaining time: <span style='display:inline-block;width:80px;'>{time_remaining}</span>
                                Estimated time: <span style='display:inline-block;width:80px;'>{time_estimated}</span>
                                </insite:ProgressStatus>
                                <insite:ProgressStatus Text="Speed: {upload_speed}" />
                            </Items>
                        </insite:ProgressPanel>

                        <script type="text/javascript">
                            (function () {
                                var instance = window.progressTest = window.progressTest || {};

                                instance.onPollStart = function () {
                                    console.log(this.id + ': onPollStart');
                                };
                                instance.onPollError = function () {
                                    console.log(this.id + ': onPollError');
                                };
                                instance.onPollStopped = function () {
                                    console.log(this.id + ': onPollStopped');
                                };
                                instance.onCancelled = function () {
                                    console.log(this.id + ': onCancelled');
                                };
                                instance.onSubmitStart = function () {
                                    console.log(this.id + ': onSubmitStart');
                                };
                                instance.onSubmitError = function () {
                                    console.log(this.id + ': onSubmitError');
                                };

                            })();
                        </script>

                    </section>

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" Title="Button">

            <h2>Buttons</h2>

            <p>
                Solid <a href="<%= AroundHomeUrl %>/components/buttons.html">buttons</a> are preferred, and a button should
                        include both icon and text.
            </p>

            <pre><code>
    &lt;insite:Button runat="server" Icon="fas fa-trash-alt" Text="Delete" ButtonStyle="Danger" /&gt;
    &lt;insite:Button runat="server" Icon="fas fa-cloud-upload" Text="Save" ButtonStyle="Success" /&gt;
                    </code></pre>

            <h3 class="mt-3">Examples</h3>

            <insite:Button runat="server" ID="EditButton" Icon="fas fa-edit" Text="Edit" ButtonStyle="Primary" />
            <insite:Button runat="server" ID="SaveButton" Icon="fas fa-cloud-upload" Text="Save" ButtonStyle="Success" />
            <insite:Button runat="server" ID="DeleteButton" Icon="fas fa-trash-alt" Text="Delete" ButtonStyle="Danger" />
            <insite:Button runat="server" ID="PrintButton" Icon="fas fa-print" Text="Print" ButtonStyle="Default" />

            <asp:Literal runat="server" ID="ButtonOutput" />

            <div>
                <b style="color: green;">Green buttons</b>
                <pre>ButtonStyle="Success"</pre>
                <table>
                    <tr>
                        <th>Button</th>
                        <th>Action</th>
                        <th>Icon</th>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Text="Save" Icon="fas fa-cloud-upload" ButtonStyle="Success" />
                        </td>
                        <td>
                            <p>Save</p>
                            <p>Publish</p>
                        </td>
                        <td>fas fa-cloud-upload</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Text="Apply" Icon="fas fa-check" ButtonStyle="Success" />
                        </td>
                        <td>
                            <p>Yes</p>
                            <p>Apply</p>
                        </td>
                        <td>fas fa-check</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Success" Text="Upload" Icon="fas fa-upload" />
                        </td>
                        <td>
                            <p>Upload</p>
                            <p>Publish</p>
                        </td>
                        <td>fas fa-upload</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Success" Text="Submit to DA" Icon="fas fa-share-square" />
                        </td>
                        <td>Submit to DA</td>
                        <td>fas fa-share-square</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Text="Add" Icon="fas fa-plus-circle" ButtonStyle="Success" />
                        </td>
                        <td>Add</td>
                        <td>fas fa-plus-circle</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Success" Text="Register" Icon="fa fa-user-plus" />
                        </td>
                        <td>Register</td>
                        <td>fa fa-user-plus</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Success" Text="Load All" Icon="fas fa-spinner" />
                        </td>
                        <td>Load All</td>
                        <td>fas fa-spinner</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Success" Text="Recode" Icon="fas fa-sort-numeric-down" />
                        </td>
                        <td>Recode</td>
                        <td>fas fa-sort-numeric-down</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Text="Unlock" Icon="far fa-lock-open" ButtonStyle="Success" />
                        </td>
                        <td>Unlock</td>
                        <td>far fa-lock-open</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Text="Unarchive" Icon="fas fa-box-open" ButtonStyle="Success" />
                        </td>
                        <td>Unarchive</td>
                        <td>fas fa-box-open</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Success" Text="Compare" Icon="far fa-chart-bar" />
                        </td>
                        <td>Compare</td>
                        <td>far fa-chart-bar</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Text="Start" ButtonStyle="Success" Icon="far fa-rocket-launch" />
                        </td>
                        <td>
                            <p>Start</p>
                            <p>Launch</p>
                        </td>
                        <td>far fa-rocket-launch</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Success" Text="Place Distribution Order" Icon="fas fa-mail-bulk" />
                        </td>
                        <td>Place Distribution Order</td>
                        <td>fas fa-mail-bulk</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Success" Text="Sign In" Icon="fas fa-sign-in-alt" />
                        </td>
                        <td>Sign In</td>
                        <td>fas fa-sign-in-alt</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Success" Text="Sign Off" Icon="fas fa-check-double" />
                        </td>
                        <td>Sign Off</td>
                        <td>fas fa-check-double</td>
                    </tr>
                    <tr>
                        <td></td>
                        <td></td>
                        <td></td>
                    </tr>
                </table>
            </div>

            <div>
                <b style="color: red;">Red buttons</b>
                <pre>ButtonStyle="Danger"</pre>
                <table>
                    <tr>
                        <th>Button</th>
                        <th>Action</th>
                        <th>Icon</th>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Text="Delete" Icon="fas fa-trash-alt" ButtonStyle="Danger" />
                        </td>
                        <td>Delete</td>
                        <td>fas fa-trash-alt</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Text="Archive" Icon="fas fa-archive" ButtonStyle="Danger" />
                        </td>
                        <td>Archive</td>
                        <td>fas fa-archive</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Text="Close" Icon="fas fa-folder" ButtonStyle="Danger" />
                        </td>
                        <td>Close</td>
                        <td>fas fa-folder</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Text="Lock" Icon="fas fa-lock" ButtonStyle="Danger" />
                        </td>
                        <td>Lock</td>
                        <td>fas fa-lock</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Text="Unpublish" Icon="fas fa-eraser" ButtonStyle="Danger" />
                        </td>
                        <td>Unpublish</td>
                        <td>fas fa-eraser</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Text="Reset Password" Icon="fas fa-sync" ButtonStyle="Danger" />
                        </td>
                        <td>Reset Password</td>
                        <td>fas fa-sync</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Text="Restart" Icon="me-1 far fa-redo" ButtonStyle="Danger" />
                        </td>
                        <td>Restart</td>
                        <td>me-1 far fa-redo</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Text="Stop" ButtonStyle="Danger" Icon="fas fa-stop-circle" />
                        </td>
                        <td>Stop</td>
                        <td>fas fa-stop-circle</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ToolTip="Upgrade" ButtonStyle="Danger" Text="Upgrade" Icon="fas fa-sort-shapes-up" />
                        </td>
                        <td>Upgrade</td>
                        <td>fas fa-sort-shapes-up</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Danger" Icon="fas fa-bug" Text="Development" />
                        </td>
                        <td>
                            <p>Development</p>
                            <p>Demo</p>
                        </td>
                        <td>fas fa-bug</td>
                    </tr>
                    <tr>
                        <td></td>
                        <td></td>
                        <td></td>
                    </tr>
                </table>
            </div>

            <div>
                <b style="color: blue;">Blue buttons</b>
                <pre>ButtonStyle="Primary"</pre>
                <table>
                    <tr>
                        <th>Button</th>
                        <th>Action</th>
                        <th>Icon</th>
                    </tr>

                    <tr>
                        <td>
                            <insite:Button runat="server" Text="Filter" Icon="far fa-filter" ButtonStyle="Primary" />
                        </td>
                        <td>Filter</td>
                        <td>far fa-filter</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Text="Clear" Icon="far fa-undo" ButtonStyle="Primary" />
                        </td>
                        <td>
                            <p>Clear</p>
                            <p>Reset</p>
                        </td>
                        <td>far fa-undo</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Text="Build" Icon="far fa-chart-bar" ButtonStyle="Primary" />
                        </td>
                        <td>Build</td>
                        <td>far fa-chart-bar</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Text="Download" Icon="fas fa-download" ButtonStyle="Primary" />
                        </td>
                        <td>
                            <p>Download</p>
                            <p>Scrap Paper</p>
                        </td>
                        <td>fas fa-download</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Text="Next" Icon="fas fa-arrow-alt-right" IconPosition="AfterText" ButtonStyle="Primary" />
                        </td>
                        <td>
                            <p>Next</p>
                            <p>Continue</p>
                        </td>
                        <td>fas fa-arrow-alt-right</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Text="Watch" Icon="fas fa-video" ButtonStyle="Primary" />
                        </td>
                        <td>Watch</td>
                        <td>fas fa-video</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Text="Loading ..." Icon="fa fa-spinner fa-pulse fa-3x" IconPosition="AfterText" ButtonStyle="Primary" />
                        </td>
                        <td>Loading ...</td>
                        <td>fa fa-spinner fa-pulse fa-3x</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Text="Stop Impersonating" Icon="fas fa-power-off" ButtonStyle="Primary" />
                        </td>
                        <td>Stop Impersonating</td>
                        <td>fas fa-power-off</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Text="Release" Icon="fas fa-calendar-check" ButtonStyle="Primary" />
                        </td>
                        <td>Release (grades for exams)</td>
                        <td>fas fa-calendar-check</td>
                    </tr>
                    <tr>
                        <td></td>
                        <td></td>
                        <td></td>
                    </tr>
                </table>
            </div>

            <div>
                <b style="color: yellow;">Yellow buttons</b>
                <pre>ButtonStyle="Warning"</pre>
                <table>
                    <tr>
                        <th>Button</th>
                        <th>Action</th>
                        <th>Icon</th>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Warning" Text="Validate" Icon="fas fa-shield-check" />
                        </td>
                        <td>
                            <p>Validate</p>
                            <p>Verify</p>
                        </td>
                        <td>fas fa-shield-check</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Warning" Text="Sandbox" Icon="fas fa-presentation" />
                        </td>
                        <td>Sandbox
                        </td>
                        <td>fas fa-presentation</td>
                    </tr>

                    <tr>
                        <td></td>
                        <td></td>
                        <td></td>
                    </tr>
                </table>
            </div>

            <div>
                <b style="color: gray;">Gray buttons</b>
                <pre>ButtonStyle="None"</pre>
                <table>
                    <tr>
                        <th>Button</th>
                        <th>Action</th>
                        <th>Icon</th>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Icon="fa fa-cubes" Text="Shift iQ" ButtonStyle="None" CssClass="btn btn-gray" />
                        </td>
                        <td>Shift iQ</td>
                        <td>fa fa-cubes</td>
                    </tr>
                </table>
            </div>
            <div>
                <b>Secondary buttons</b>
                <pre>ButtonStyle="Secondary"</pre>
                <table>
                    <tr>
                        <th>Button</th>
                        <th>Action</th>
                        <th>Icon</th>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Secondary" Text="Sign Out" Icon="fas fa-sign-out" />
                        </td>
                        <td>Sign Out</td>
                        <td>fas fa-sign-out</td>
                    </tr>
                    <tr>
                        <td></td>
                        <td></td>
                        <td></td>
                    </tr>
                </table>
            </div>

            <div>
                <b>Default buttons</b>
                <pre>ButtonStyle="Default"</pre>
                <table style="padding: 8px; margin: 10px;">
                    <tr>
                        <th>Button</th>
                        <th>Action</th>
                        <th>Icon</th>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Text="Cancel" Icon="fas fa-ban" ButtonStyle="Default" />
                        </td>
                        <td>
                            <p>Cancel</p>
                            <p>Close</p>
                        </td>
                        <td>fas fa-ban</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Text="No" Icon="fas fa-times" ButtonStyle="Default" />
                        </td>
                        <td>No</td>
                        <td>fas fa-times</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Print" Icon="fas fa-print" />
                        </td>
                        <td>Print</td>
                        <td>fas fa-print</td>
                    </tr>

                    <tr>
                        <td colspan="2">Change data</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Text="Edit" Icon="fas fa-pencil" ButtonStyle="Default" />
                        </td>
                        <td>Edit</td>
                        <td>fas fa-pencil</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Duplicate" Icon="fas fa-copy" />
                        </td>
                        <td>Duplicate</td>
                        <td>fas fa-copy</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Bulk Update" Icon="fas fa-pen-square" />
                        </td>
                        <td>Bulk Update</td>
                        <td>fas fa-pen-square</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Merge" Icon="fas fa-code-merge" />
                        </td>
                        <td>Merge</td>
                        <td>fas fa-code-merge</td>
                    </tr>

                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Take Snapshot" Icon="far fa-camera" />
                        </td>
                        <td>Take Snapshot</td>
                        <td>far fa-camera</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Icon="far fa-indent" Text="Indent" />
                        </td>
                        <td>Indent</td>
                        <td>far fa-indent</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Icon="far fa-outdent" Text="Outdent" />
                        </td>
                        <td>Outdent</td>
                        <td>far fa-outdent</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Icon="far fa-sort" Text="Reorder" />
                        </td>
                        <td>Reorder</td>
                        <td>far fa-sort</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Migrate" Icon="far fa-file-export" />
                        </td>
                        <td>
                            <p>Move</p>
                            <p>Migrate</p>
                        </td>
                        <td>far fa-file-export</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Move to Addendum" Icon="fas fa-reply" />
                        </td>
                        <td>Move to Addendum</td>
                        <td>fas fa-reply</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Icon="fas fa-arrows-alt-v" Text="Cell Move" />
                        </td>
                        <td>Cell Move</td>
                        <td>fas fa-arrows-alt-v</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Jump To Question" Icon="far fa-reply fa-rotate-270" />
                        </td>
                        <td>Jump To Question</td>
                        <td>far fa-reply fa-rotate-270</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Tag" Icon="fas fa-tag" />
                        </td>
                        <td>
                            <p>Tag</p>
                            <p>Classify</p>
                        </td>
                        <td>fas fa-tag</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Calculate" Icon="fas fa-calculator" />
                        </td>
                        <td>Calculate</td>
                        <td>fas fa-calculator</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Grant Credentials" Icon="fas fa-award" />
                        </td>
                        <td>Grant Credentials</td>
                        <td>fas fa-award</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Text="Withhold Grade" Icon="fas fa-calendar-times" ButtonStyle="Default" />
                        </td>
                        <td>Withhold Grade (for exams)</td>
                        <td>fas fa-calendar-times</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Trigger" Icon="fas fa-bolt" />
                        </td>
                        <td>
                            <p>Trigger</p>
                            <p>Execute</p>
                        </td>
                        <td>fas fa-bolt</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Schedule" Icon="fas fa-calendar-alt" />
                        </td>
                        <td>Schedule</td>
                        <td>fas fa-calendar-alt</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Translate" Icon="fas fa-globe" />
                        </td>
                        <td>Translate</td>
                        <td>fas fa-globe</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Send Email" Icon="fas fa-paper-plane" />
                        </td>
                        <td>
                            <p>Publish Schedule</p>
                            <p>Send</p>
                            <p>Submit</p>
                            <p>Request</p>
                        </td>
                        <td>fas fa-paper-plane</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Impersonate" Icon="fas fa-user-secret" />
                        </td>
                        <td>Impersonate</td>
                        <td>fas fa-user-secret</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Synchronize" Icon="fas fa-exchange" />
                        </td>
                        <td>
                            <p>Synchronize</p>
                            <p>Swap</p>
                        </td>
                        <td>fas fa-exchange</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Bookmark" Icon="far fa-bookmark" />
                        </td>
                        <td>Bookmark</td>
                        <td>far fa-bookmark</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Grant Permission" Icon="far fa-key" />
                        </td>
                        <td>Grant Permission</td>
                        <td>far fa-key</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Link Term" Icon="fas fa-link" />
                        </td>
                        <td>Link Term</td>
                        <td>fas fa-link</td>
                    </tr>

                    <tr>
                        <td colspan="2">Navigate</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Preview" Icon="fas fa-external-link" />
                        </td>
                        <td>Preview</td>
                        <td>fas fa-external-link</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Review" Icon="fas fa-search" />
                        </td>
                        <td>Review</td>
                        <td>fas fa-search</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Icon="fas fa-arrow-alt-left" Text="Back" />
                        </td>
                        <td>
                            <p>Previous</p>
                            <p>Back</p>
                        </td>
                        <td>fas fa-arrow-alt-left</td>
                    </tr>

                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Icon="fas fa-arrow-up" Text="Top" />
                        </td>
                        <td>Top</td>
                        <td>fas fa-arrow-up</td>
                    </tr>

                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Home" Icon="fas fa-home" />
                        </td>
                        <td>Home</td>
                        <td>fas fa-home</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Icon="fas fa-chevron-down" Text="Expand All" />
                        </td>
                        <td>Expand All</td>
                        <td>fas fa-chevron-down</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Icon="fas fa-chevron-up" Text="Collapse All" />
                        </td>
                        <td>Collapse All</td>
                        <td>fas fa-chevron-up</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Icon="fas fa-plus" Text="Expand" />
                        </td>
                        <td>Expand</td>
                        <td>fas fa-plus</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Icon="fas fa-minus" Text="Collapse" />
                        </td>
                        <td>Collapse</td>
                        <td>fas fa-minus</td>
                    </tr>

                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Icon="far fa-eye-slash" Text="Hide Legend" />
                        </td>
                        <td>Hide Legend</td>
                        <td>far fa-eye-slash</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Icon="far fa-eye" Text="Show Legend" />
                        </td>
                        <td>Show Legend</td>
                        <td>far fa-eye</td>
                    </tr>

                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Icon="fa fa-search-plus" Text="Zoom In" />
                        </td>
                        <td>Zoom In</td>
                        <td>fa fa-search-plus</td>
                    </tr>

                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Icon="fa fa-search-minus" Text="Zoom Out" />
                        </td>
                        <td>Zoom Out</td>
                        <td>fa fa-search-minus</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Icon="fa fa-search" Text="Actual Size" />
                        </td>
                        <td>
                            <p>Actual Size</p>
                            <p>Review (Feedback)</p>
                        </td>
                        <td>fa fa-search</td>
                    </tr>

                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Select All" Icon="far fa-square" />
                        </td>
                        <td>Select All</td>
                        <td>far fa-square</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Deselect All" Icon="far fa-check-square" />
                        </td>
                        <td>Deselect All</td>
                        <td>far fa-check-square</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="History" Icon="fas fa-history" />
                        </td>
                        <td>History</td>
                        <td>fas fa-history</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Outline" Icon="fas fa-sitemap" />
                        </td>
                        <td>Outline</td>
                        <td>fas fa-sitemap</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="View Details" Icon="fa fa-bars" />
                        </td>
                        <td>View Details</td>
                        <td>fa fa-bars</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="View Chart" Icon="fa fa-chart-line" />
                        </td>
                        <td>View Chart</td>
                        <td>fa fa-chart-line</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Support" Icon="fas fa-envelope" />
                        </td>
                        <td>Support</td>
                        <td>fas fa-envelope</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Grade" Icon="fas fa-spell-check" />
                        </td>
                        <td>Grade</td>
                        <td>fas fa-spell-check</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Workshop" Icon="far fa-industry-alt" />
                        </td>
                        <td>Workshop</td>
                        <td>far fa-industry-alt</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Request" Icon="far fa-plug" />
                        </td>
                        <td>Request</td>
                        <td>far fa-plug</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Report" Icon="fas fa-chart-bar" />
                        </td>
                        <td>Report</td>
                        <td>fas fa-chart-bar</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Excel report (*.xls)" Icon="fas fa-file-excel" />
                        </td>
                        <td>Excel report (*.xls)</td>
                        <td>fas fa-file-excel</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Text (.csv)" Icon="fas fa-file-csv" />
                        </td>
                        <td>Text (.csv)</td>
                        <td>fas fa-file-csv</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="PDF Document" Icon="fas fa-file-pdf" />
                        </td>
                        <td>PDF Document</td>
                        <td>fas fa-file-pdf</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" Text="Word document" Icon="far fa-file-word" ButtonStyle="Default" />
                        </td>
                        <td>Word document
                        </td>
                        <td>far fa-file-word</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="XML Report (*.xml)" Icon="far fa-file-code" />
                        </td>
                        <td>XML Report (*.xml)</td>
                        <td>far fa-file-code</td>
                    </tr>
                    <tr>
                        <td>
                            <insite:Button runat="server" ButtonStyle="Default" Text="Images" Icon="fas fa-image" />
                        </td>
                        <td>Images</td>
                        <td>fas fa-image</td>
                    </tr>
                    <tr>
                        <td></td>
                        <td></td>
                        <td></td>
                    </tr>

                </table>
            </div>

        </insite:NavItem>

        <insite:NavItem runat="server" Title="Input">

            <h3>Input Types</h3>

            <p>
                Basic input types used in our application <a href="<%= AroundHomeUrl %>/components/forms.html">forms</a>
                are as follows:
            </p>

            <pre><code>
&lt;insite:CheckBox runat="server" Text="CheckBox" /&gt;
&lt;insite:DateSelector runat="server" /&gt;
&lt;insite:TextBox runat="server" TextMode="Email" EmptyMessage="example@domain.com" /&gt;
&lt;insite:NumericBox runat="server" /&gt;
&lt;insite:RadioButton runat="server" Text="Organization" /&gt;
&lt;insite:ComboBox runat="server"&gt;
    &lt;insite:ComboBoxOption Value="12" Text="Apples" /&gt;
    &lt;insite:ComboBoxOption Value="7" Text="Oranges" /&gt;
    &lt;insite:ComboBoxOption Value="88" Text="Watermelons" Selected="true" /&gt;
&lt;/insite:ComboBox&gt;
&lt;insite:CheckSwitch runat="server" Text="Toggle" /&gt;
&lt;insite:TextBox runat="server" EmptyMessage="Enter your text here" MaxLength="40" /&gt;
&lt;insite:InputFilter runat="server" Placeholder="Search keyword" /&gt;
                </code></pre>

            <h3 class="mt-3">Examples</h3>

            <div class="row">
                <div class="col-md-6">
                    <insite:CheckBox runat="server" Text="CheckBox" />
                    Check
                    <br />
                    <br />
                    <insite:DateSelector runat="server" />
                    Date<br />
                    <br />
                    <insite:TextBox runat="server" TextMode="Email" EmptyMessage="example@domain.com" />
                    Email<br />
                    <br />
                    <insite:NumericBox runat="server" />
                    Number<br />
                    <br />
                    <insite:RadioButton runat="server" Text="Organization" />
                    Radio<br />
                    <br />
                    <insite:ComboBox runat="server">
                        <Items>
                            <insite:ComboBoxOption Value="12" Text="Apples" />
                            <insite:ComboBoxOption Value="7" Text="Oranges" />
                            <insite:ComboBoxOption Value="88" Text="Watermelons" Selected="true" />
                        </Items>
                    </insite:ComboBox>
                    Select<br />
                    <br />
                    <insite:CheckSwitch runat="server" Text="Toggle" />
                    Switch<br />
                    <br />
                    <insite:TextBox runat="server" EmptyMessage="Enter your text here" MaxLength="40" />
                    Text<br />
                    <br />
                    <insite:InputFilter runat="server" EmptyMessage="Search keyword" />
                    Search keyword
                </div>
            </div>

        </insite:NavItem>

        <insite:NavItem runat="server" Title="InputAudio">
            <div class="row">
                <div class="col-lg-9 content py-4">
                    <section class="pb-5">

                        <h2>InputAudio</h2>
                        <p>Use <strong>InputAudio</strong> control when you need to record audio.</p>

                        <pre><code>
    &lt;insite:InputAudio runat="server" ID="AudioRecorder" AutoPostBack="true" /&gt;
                        </code></pre>

                        <h3 class="mt-3">Setup</h3>

                        <p>
                            The control depends on extrnal tool: <a href="https://ffmpeg.org">FFmpeg</a>.
                            The tool is used to validate the media data on the server side.
                            The path to the tool is determined by <strong>AppSettings.Integration.FFmpeg.ToolPath</strong> property.
                        </p>
                        <p>
                            Compiled binary files of the tool can be found on this page: <a href="https://github.com/BtbN/FFmpeg-Builds/releases">https://github.com/BtbN/FFmpeg-Builds/releases</a>.
                            Go to the <i>Latest Auto-Build</i> section, click <i>Show all assets</i> link and find the latest release build for <b>Win64</b> platform licensed under <b>LGPL</b> license.
                            The control functionality was tested using <i>FFmpeg v6.1</i> with <u>shared</u> binaries so the archive name is <i>ffmpeg-n<b>6.1</b>-latest-<b>win64</b>-<b>lgpl</b>-<u>shared</u>-6.1.zip</i>.
                            <br />
                            Download the archive, open it, go to <b>bin</b> directory and copy all the files placed here to the tool's folder. For example, here it the list of files:
                        </p>
                        <ul>
                            <li>avcodec-60.dll</li>
                            <li>avdevice-60.dll</li>
                            <li>avfilter-9.dll</li>
                            <li>avformat-60.dll</li>
                            <li>avutil-58.dll</li>
                            <li><b>ffmpeg.exe</b></li>
                            <li>ffplay.exe</li>
                            <li><b>ffprobe.exe</b></li>
                            <li>swresample-4.dll</li>
                            <li>swscale-7.dll</li>
                        </ul>
                        
                        <p>The setup is done.</p>

                        <h3 class="mt-3">Server-side properties</h3>

                        <ul>
                            <li>
                                <strong>AllowPause</strong> determines whether the <i>Pause</i> button is visible.

                                <table class="mt-3">
                                    <tr>
                                        <td class="p-2 font-monospace">AllowPause = true</td>
                                        <td class="p-2 font-monospace">AllowPause = false</td>
                                    </tr>
                                    <tr>
                                        <td class="p-2"><insite:InputAudio runat="server" AllowPause="true" AutoUpload="false" /></td>
                                        <td class="p-2"><insite:InputAudio runat="server" AllowPause="false" AutoUpload="false" /></td>
                                    </tr>
                                </table>
                            </li>
                            <li>
                                <strong>AttemptLimit</strong> defines the maximum number of recording attempts.
                                If the value is zero then the attempt limitation is disabled.

                                <table class="mt-3">
                                    <tr>
                                        <td class="p-2 font-monospace">AttemptLimit = 0</td>
                                        <td class="p-2 font-monospace">AttemptLimit = 3</td>
                                    </tr>
                                    <tr>
                                        <td class="p-2"><insite:InputAudio runat="server" AttemptLimit="0" AutoUpload="false" /></td>
                                        <td class="p-2"><insite:InputAudio runat="server" AttemptLimit="3" AutoUpload="false" /></td>
                                    </tr>
                                </table>
                            </li>
                            <li>
                                <strong>AutoPostBack</strong> defines if the control should automatically 
                                posts back to the server after media data was uploaded. 
                                If <i>AutoPostBack = true</i> but <i>AutoUpload = false</i> then the control 
                                will do post back only after the media data is uploaded using JS code.

                                <table class="mt-3">
                                    <tr>
                                        <td class="p-2 font-monospace">AutoPostBack = true</td>
                                        <td class="p-2 font-monospace">AutoPostBack = false</td>
                                    </tr>
                                    <tr>
                                        <td class="p-2"><insite:InputAudio runat="server" UploadMode="PostBack" AutoPostBack="true" /></td>
                                        <td class="p-2"><insite:InputAudio runat="server" UploadMode="PostBack" AutoPostBack="false" /></td>
                                    </tr>
                                </table>
                            </li>
                            <li>
                                <strong>AutoUpload</strong> defines if the control should automatically upload the media data.

                                <table class="mt-3">
                                    <tr>
                                        <td class="p-2 font-monospace">AutoUpload = true</td>
                                        <td class="p-2 font-monospace">AutoUpload = false</td>
                                    </tr>
                                    <tr>
                                        <td class="p-2"><insite:InputAudio runat="server" UploadMode="API" AutoUpload="true" AutoPostBack="true" /></td>
                                        <td class="p-2"><insite:InputAudio runat="server" UploadMode="API" AutoUpload="false" AutoPostBack="true" /></td>
                                    </tr>
                                </table>
                            </li>
                            <li>
                                <strong>Bitrate</strong> defines the bitrate of recording. It affects the quality of audio record and the final size of media data. Available <i>BitrateMode</i> enumeration values:
                                <ul>
                                    <li><strong>kb_8</strong> = 8 kbit/s</li>
                                    <li><strong>kb_16</strong> = 16 kbit/s</li>
                                    <li><strong>kb_32</strong> = 32 kbit/s</li>
                                    <li><strong>kb_64</strong> = 64 kbit/s (DEFAULT)</li>
                                    <li><strong>kb_128</strong> = 128 kbit/s</li>
                                    <li><strong>kb_256</strong> = 256 kbit/s</li>
                                </ul>

                                <insite:UpdatePanel runat="server">
                                    <ContentTemplate>
                                        <table class="mt-3">
                                            <tr>
                                                <td class="p-2" colspan="2">
                                                    <insite:Alert runat="server" ID="AudioInputBitrateTestStatus" Text="Click the Start button and wait until it stops." Indicator="Information" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="p-2 align-top"><insite:ComboBox runat="server" ID="AudioInputBitrateTestSelector" AllowBlank="false" /></td>
                                                <td class="p-2 align-top"><insite:InputAudio runat="server" ID="AudioInputBitrateTestInput" TimeLimit="5" AutoPostBack="true" UploadMode="API" /></td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </insite:UpdatePanel>
                            </li>
                            <li>
                                <strong>CurrentAttempt</strong> defines the initial number of recorded attempts.

                                <table class="mt-3">
                                    <tr>
                                        <td class="p-2 font-monospace">CurrentAttempt = 1<br />AttemptLimit = 3</td>
                                        <td class="p-2 font-monospace">CurrentAttempt = 4<br />AttemptLimit = 3</td>
                                    </tr>
                                    <tr>
                                        <td class="p-2"><insite:InputAudio runat="server" AttemptLimit="3" CurrentAttempt="1" AutoUpload="false" /></td>
                                        <td class="p-2"><insite:InputAudio runat="server" AttemptLimit="3" CurrentAttempt="4" AutoUpload="false" /></td>
                                    </tr>
                                </table>
                            </li>
                            <li>
                                <strong>Enabled</strong> defines initial state of the control's <i>Start</i> button.
                                The button's state can be changed on the client-side.
                                The server control still able to get media data.

                                <table class="mt-3">
                                    <tr>
                                        <td class="p-2 font-monospace">Enabled = true</td>
                                        <td class="p-2 font-monospace">Enabled = false</td>
                                    </tr>
                                    <tr>
                                        <td class="p-2"><insite:InputAudio runat="server" Enabled="true" AutoUpload="false" /></td>
                                        <td class="p-2"><insite:InputAudio runat="server" Enabled="false" AutoUpload="false" /></td>
                                    </tr>
                                </table>
                            </li>
                            <li>
                                <strong>ReadOnly</strong> similar to functionality of <i>Enabled</i> property but
                                if the value was set to <i>true</i> then the Start button can't be enabled on the client-side
                                and the server control will ignore received media data. The control automatically goes to the read-only
                                state when <i>CurrentAttempt >= AttemptLimit</i>.

                                <table class="mt-3">
                                    <tr>
                                        <td class="p-2 font-monospace">ReadOnly = true</td>
                                        <td class="p-2 font-monospace">ReadOnly = false</td>
                                    </tr>
                                    <tr>
                                        <td class="p-2"><insite:InputAudio runat="server" ReadOnly="true" AutoUpload="false" /></td>
                                        <td class="p-2"><insite:InputAudio runat="server" ReadOnly="false" AutoUpload="false" /></td>
                                    </tr>
                                </table>
                            </li>
                            <li>
                                <strong>TimeLimit</strong> defines the recording time limit in seconds.
                                If defined, the timer will run backwards.
                                If not defined, the timer will run forwards and the recording time will be
                                limited to <b>99 minutes, 59 seconds</b>.

                                <table class="mt-3">
                                    <tr>
                                        <td class="p-2 font-monospace">TimeLimit = 30</td>
                                        <td class="p-2 font-monospace">TimeLimit = 10000</td>
                                    </tr>
                                    <tr>
                                        <td class="p-2"><insite:InputAudio runat="server" TimeLimit="30" /></td>
                                        <td class="p-2"><insite:InputAudio runat="server" TimeLimit="10000" /></td>
                                    </tr>
                                </table>
                            </li>
                            <li>
                                <strong>UploadMode</strong> defines in which way the control should upload the media data:
                                <ul>
                                    <li><strong>API</strong> upload the media data to the server through <i>AssetsController</i></li>
                                    <li><strong>PostBack</strong> upload the media data through post back to the server</li>
                                </ul>

                                <table class="mt-3">
                                    <tr>
                                        <td class="p-2 font-monospace">UploadMode = API</td>
                                        <td class="p-2 font-monospace">UploadMode = PostBack</td>
                                    </tr>
                                    <tr>
                                        <td class="p-2"><insite:InputAudio runat="server" UploadMode="API" AutoPostBack="true" /></td>
                                        <td class="p-2"><insite:InputAudio runat="server" UploadMode="PostBack" AutoPostBack="true" /></td>
                                    </tr>
                                </table>
                            </li>
                        </ul>

                        <h3 class="mt-3">Client-side</h3>
                        
                        <div class="my-3">
                            <insite:InputAudio runat="server" ID="InputAudioClientSide" TimeLimit="10" UploadMode="PostBack" AutoUpload="false" AutoPostBack="false" />
                        </div>

                        <table class="mt-3">
                            <tr>
                                <td class="p-2 align-top">
                                    <h6>Methods</h6>
                                    <div class="btn-group-vertical">
                                        <insite:Button runat="server" Text="el.inputAudio.start()" OnClientClick='inputAudioTest.start(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.inputAudio.stop()" OnClientClick='inputAudioTest.stop(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.inputAudio.pause()" OnClientClick='inputAudioTest.pause(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.inputAudio.resume()" OnClientClick='inputAudioTest.resume(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.inputAudio.clear()" OnClientClick='inputAudioTest.clear(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.inputAudio.upload()" OnClientClick='inputAudioTest.upload(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.inputAudio.submit()" OnClientClick='inputAudioTest.submit(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.inputAudio.disable()" OnClientClick='inputAudioTest.disable(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.inputAudio.enable()" OnClientClick='inputAudioTest.enable(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.inputAudio.show()" OnClientClick='inputAudioTest.show(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.inputAudio.hide()" OnClientClick='inputAudioTest.hide(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.inputAudio.disableButton(all)" OnClientClick='inputAudioTest.disableButton(inSite.common.inputAudio.button.all); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.inputAudio.disableButton(startStop)" OnClientClick='inputAudioTest.disableButton(inSite.common.inputAudio.button.startStop); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.inputAudio.disableButton(playPause)" OnClientClick='inputAudioTest.disableButton(inSite.common.inputAudio.button.playPause); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.inputAudio.enableButton(all)" OnClientClick='inputAudioTest.enableButton(inSite.common.inputAudio.button.all); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.inputAudio.enableButton(startStop)" OnClientClick='inputAudioTest.enableButton(inSite.common.inputAudio.button.startStop); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.inputAudio.enableButton(playPause)" OnClientClick='inputAudioTest.enableButton(inSite.common.inputAudio.button.playPause); return false;' ButtonStyle="Default" />
                                    </div>
                                </td>
                                <td class="p-2 align-top">
                                    <h6>Properties</h6>
                                    <div class="btn-group-vertical">
                                        <insite:Button runat="server" Text="el.inputAudio.state" OnClientClick='inputAudioTest.state(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.inputAudio.timeLimit" OnClientClick='inputAudioTest.timeLimit(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.inputAudio.attemptLimit" OnClientClick='inputAudioTest.attemptLimit(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.inputAudio.attemptNow" OnClientClick='inputAudioTest.attemptNow(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.inputAudio.mimeType" OnClientClick='inputAudioTest.mimeType(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.inputAudio.fileName" OnClientClick='inputAudioTest.fileName(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.inputAudio.mediaData" OnClientClick='inputAudioTest.mediaData(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.inputAudio.isDisabled" OnClientClick='inputAudioTest.isDisabled(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.inputAudio.isReadOnly" OnClientClick='inputAudioTest.isReadOnly(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.inputAudio.isButtonDisabled(all)" OnClientClick='inputAudioTest.isButtonDisabled(inSite.common.inputAudio.button.all); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.inputAudio.isButtonDisabled(startStop)" OnClientClick='inputAudioTest.isButtonDisabled(inSite.common.inputAudio.button.startStop); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.inputAudio.isButtonDisabled(playPause)" OnClientClick='inputAudioTest.isButtonDisabled(inSite.common.inputAudio.button.playPause); return false;' ButtonStyle="Default" />
                                    </div>
                                </td>
                                <td class="p-2 align-top" style="min-width:500px;">
                                    <h6>Output</h6>
                                    <insite:TextBox runat="server" ID="InputAudioClientOutput" ReadOnly="true" />
                                    <h6 class="mt-3">Events</h6>
                                    <insite:TextBox runat="server" ID="InputAudioClientEvents" ReadOnly="true" TextMode="MultiLine" Rows="10" />
                                </td>
                            </tr>
                        </table>

                        <script type="text/javascript">
                            (function () {
                                const element = <%= InputAudioClientSide.ClientID %>;
                                const elOutput =  <%= InputAudioClientOutput.ClientID %>;
                                const elEvents =  <%= InputAudioClientEvents.ClientID %>;

                                window.inputAudioTest = {
                                    start: function () { element.inputAudio.start(); },
                                    stop: function () { element.inputAudio.stop(); },
                                    pause: function () { element.inputAudio.pause(); },
                                    resume: function () { element.inputAudio.resume(); },
                                    clear: function () { element.inputAudio.clear(); },
                                    upload: function () { element.inputAudio.upload(); },
                                    submit: function () { element.inputAudio.submit(); },
                                    disable: function () { element.inputAudio.disable(); },
                                    enable: function () { element.inputAudio.enable(); },
                                    disableButton: function (id) { element.inputAudio.disableButton(id); },
                                    enableButton: function (id) { element.inputAudio.enableButton(id); },
                                    show: function () { element.inputAudio.show(); },
                                    hide: function () { element.inputAudio.hide(); },

                                    state: function () { output('state'); },
                                    timeLimit: function () { output('timeLimit'); },
                                    attemptLimit: function () { output('attemptLimit'); },
                                    attemptNow: function () { output('attemptNow'); },
                                    mimeType: function () { output('mimeType'); },
                                    fileName: function () { output('fileName'); },
                                    mediaData: function () { output('mediaData'); },
                                    isDisabled: function () { output('isDisabled'); },
                                    isReadOnly: function () { output('isReadOnly'); },
                                    isButtonDisabled: function (id) {
                                        let name = 'unknown';

                                        const obj = inSite.common.inputAudio.button;
                                        for (let n in obj) {
                                            if (obj.hasOwnProperty(n) && obj[n] === id) {
                                                name = n;
                                                break;
                                            }
                                        }

                                        elOutput.value = name + ' = ' + String(element.inputAudio.isButtonDisabled(id));
                                    }
                                };

                                {
                                    let allEvents = '';
                                    for (var en in inSite.common.inputAudio.event) {
                                        if (inSite.common.inputAudio.event.hasOwnProperty(en)) {
                                            if (allEvents.length > 0)
                                                allEvents += ' ';

                                            allEvents += inSite.common.inputAudio.event[en];
                                        }
                                    }

                                    $(element)
                                        .on(allEvents, function (e) {
                                            elEvents.value = e.type + '\r\n' + elEvents.value;
                                        });
                                }

                                function output(name) {
                                    var value = element.inputAudio[name];
                                    if (value instanceof Blob)
                                        value = 'Blob { size: ' + String(value.size) + ', type: ' + String(value.type) + ' }';
                                    elOutput.value = name + ' = ' + String(value);
                                }
                            })();
                        </script>

                        <h3 class="mt-3">Validation</h3>

                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Required Audio
                                        <insite:RequiredValidator runat="server" ControlToValidate="InputAudioValidationTest_Audio" FieldName="Required Audio" ValidationGroup="InputAudioValidationTest" />
                                    </label>
                                    <insite:InputAudio runat="server" ID="InputAudioValidationTest_Audio" TimeLimit="10" UploadMode="PostBack" AutoPostBack="true" CausesValidation="true" ValidationGroup="InputAudioValidationTest" />
                                </div>
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Required Text
                                        <insite:RequiredValidator runat="server" ControlToValidate="InputAudioValidationTest_Text" FieldName="Required Text" ValidationGroup="InputAudioValidationTest" />
                                    </label>
                                    <insite:TextBox runat="server" ID="InputAudioValidationTest_Text" />
                                </div>
                            </div>
                        </div>

                        <div class="mt-3">
                            <insite:SaveButton runat="server" Text="Submit" CausesValidation="true" ValidationGroup="InputAudioValidationTest" />
                        </div>
                        
                    </section>
                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" Title="OutputAudio">
            <div class="row">
                <div class="col-lg-9 content py-4">
                    <section class="pb-5">

                        <h2>OutputAudio</h2>
                        <p>Use <strong>InputAudio</strong> control when you need to playback an audio.</p>

                        <pre><code>
    &lt;insite:OutputAudio runat="server" ID="AudioPlayer" /&gt;
                        </code></pre>

                        <h3 class="mt-3">Server-side properties</h3>

                        <ul>
                            <li>
                                <strong>AllowDelete</strong> determines whether the <i>Delete</i> button is visible.

                                <div class="p-2 font-monospace">AllowDelete = true</div>
                                <div class="p-2"><insite:OutputAudio runat="server" ID="OutputAudioAllowDelete" AllowDelete="true" /></div>
                                <div class="p-2 font-monospace">AllowDelete = false</div>
                                <div class="p-2"><insite:OutputAudio runat="server" AllowDelete="false" /></div>

                                <script type="text/javascript">
                                    (function () {
                                        $('#<%= OutputAudioAllowDelete.ClientID %>').on(inSite.common.outputAudio.event.delete, function (e) {
                                            e.preventDefault();

                                            alert('Delete clicked');
                                        })
                                    })();
                                </script>
                            </li>
                            <li>
                                <strong>AttemptLimit</strong> defines the maximum number of attempts.
                                If the value is zero then the <i>Attempts</i> section is hidden. The <i>Attempts</i> is an informational section only.

                                <div class="p-2 font-monospace">AttemptLimit = 0</div>
                                <div class="p-2"><insite:OutputAudio runat="server" AttemptLimit="0" /></div>
                                <div class="p-2 font-monospace">AttemptLimit = 3</div>
                                <div class="p-2"><insite:OutputAudio runat="server" AttemptLimit="3" /></div>
                            </li>
                            <li>
                                <strong>AudioURL</strong> defines the URL for media data.

                                <div class="p-2 font-monospace">AudioURL = "/UI/Admin/Foundations/AudioSample.mp3"</div>
                                <div class="p-2"><insite:OutputAudio runat="server" AudioURL="/UI/Admin/Foundations/AudioSample.mp3" /></div>
                                <div class="p-2 font-monospace">AudioURL = NULL</div>
                                <div class="p-2"><insite:OutputAudio runat="server" /></div>
                            </li>
                            <li>
                                <strong>AutoLoad</strong> defines if the media data will be loaded right after intitialization.

                                <div class="p-2 font-monospace">AutoLoad = true</div>
                                <div class="p-2"><insite:OutputAudio runat="server" AutoLoad="true" AudioURL="/UI/Admin/Foundations/AudioSample.mp3" /></div>
                                <div class="p-2 font-monospace">AutoLoad = false</div>
                                <div class="p-2"><insite:OutputAudio runat="server" AutoLoad="false" AudioURL="/UI/Admin/Foundations/AudioSample.mp3" /></div>
                            </li>
                            <li>
                                <strong>CurrentAttempt</strong> defines the number of attempts in <i>Attempts</i> section. The <i>Attempts</i> is an informational section only.

                                <div class="p-2 font-monospace">CurrentAttempt = 1<br />AttemptLimit = 3</div>
                                <div class="p-2"><insite:OutputAudio runat="server" AttemptLimit="3" CurrentAttempt="1" /></div>
                                <div class="p-2 font-monospace">CurrentAttempt = 4<br />AttemptLimit = 3</div>
                                <div class="p-2"><insite:OutputAudio runat="server" AttemptLimit="3" CurrentAttempt="4" /></div>
                            </li>
                            <li>
                                <strong>Enabled</strong> defines initial state of the control buttons.
                                The button's state can be changed on the client-side.

                                <div class="p-2 font-monospace">Enabled = true</div>
                                <div class="p-2"><insite:OutputAudio runat="server" AllowDelete="true" Enabled="true" AudioURL="/UI/Admin/Foundations/AudioSample.mp3" /></div>
                                <div class="p-2 font-monospace">Enabled = false</div>
                                <div class="p-2"><insite:OutputAudio runat="server" AllowDelete="true" Enabled="false" AudioURL="/UI/Admin/Foundations/AudioSample.mp3" /></div>
                            </li>
                            <li>
                                <strong>Muted</strong> defines initial state of <i>Mute</i> button. The button's state can be changed on the client-side.

                                <div class="p-2 font-monospace">Muted = true</div>
                                <div class="p-2"><insite:OutputAudio runat="server" Muted="true" AudioURL="/UI/Admin/Foundations/AudioSample.mp3" /></div>
                                <div class="p-2 font-monospace">Muted = false</div>
                                <div class="p-2"><insite:OutputAudio runat="server" Muted="false" AudioURL="/UI/Admin/Foundations/AudioSample.mp3" /></div>
                            </li>
                            <li>
                                <strong>Volume</strong> defines initial state of <i>Volume</i> slider. The slider's state can be changed on the client-side.

                                <div class="p-2 font-monospace">Volume = 0.25</div>
                                <div class="p-2"><insite:OutputAudio runat="server" Volume="0.25" AudioURL="/UI/Admin/Foundations/AudioSample.mp3" /></div>
                                <div class="p-2 font-monospace">Volume = 1.00</div>
                                <div class="p-2"><insite:OutputAudio runat="server" Volume="1" AudioURL="/UI/Admin/Foundations/AudioSample.mp3" /></div>
                            </li>
                        </ul>

                        <h3 class="mt-3">Client-side</h3>
                        
                        <div class="my-3">
                            <insite:OutputAudio runat="server" ID="OutputAudioClientSide" AudioURL="/UI/Admin/Foundations/AudioSample.mp3" />
                        </div>

                        <table class="mt-3">
                            <tr>
                                <td class="p-2 align-top">
                                    <h6>Methods</h6>
                                    <div class="btn-group-vertical">
                                        <insite:Button runat="server" Text="el.outputAudio.loadData()" OnClientClick='outputAudioTest.loadData(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.outputAudio.play()" OnClientClick='outputAudioTest.play(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.outputAudio.pause()" OnClientClick='outputAudioTest.pause(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.outputAudio.stop()" OnClientClick='outputAudioTest.stop(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.outputAudio.disable()" OnClientClick='outputAudioTest.disable(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.outputAudio.enable()" OnClientClick='outputAudioTest.enable(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.outputAudio.delete()" OnClientClick='outputAudioTest.delete(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.outputAudio.mute()" OnClientClick='outputAudioTest.mute(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.outputAudio.unmute()" OnClientClick='outputAudioTest.unmute(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.outputAudio.show()" OnClientClick='outputAudioTest.show(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.outputAudio.hide()" OnClientClick='outputAudioTest.hide(); return false;' ButtonStyle="Default" />
                                    </div>
                                </td>
                                <td class="p-2 align-top">
                                    <h6>Properties</h6>
                                    <div class="btn-group-vertical">
                                        <insite:Button runat="server" Text="el.outputAudio.state" OnClientClick='outputAudioTest.state(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.outputAudio.volume" OnClientClick='outputAudioTest.volume(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.outputAudio.volume = random" OnClientClick='outputAudioTest.setVolume(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.outputAudio.position" OnClientClick='outputAudioTest.position(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.outputAudio.duration" OnClientClick='outputAudioTest.duration(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.outputAudio.currentTime" OnClientClick='outputAudioTest.currentTime(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.outputAudio.attemptLimit" OnClientClick='outputAudioTest.attemptLimit(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.outputAudio.attemptLimit = random" OnClientClick='outputAudioTest.setAttemptLimit(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.outputAudio.attemptNow" OnClientClick='outputAudioTest.attemptNow(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.outputAudio.attemptNow = random" OnClientClick='outputAudioTest.setAttemptNow(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.outputAudio.isEnabled" OnClientClick='outputAudioTest.isEnabled(); return false;' ButtonStyle="Default" />
                                        <insite:Button runat="server" Text="el.outputAudio.isMuted" OnClientClick='outputAudioTest.isMuted(); return false;' ButtonStyle="Default" />
                                    </div>
                                </td>
                                <td class="p-2 align-top" style="min-width:500px;">
                                    <h6>Output</h6>
                                    <insite:TextBox runat="server" ID="OutputAudioClientSideOutput" ReadOnly="true" />
                                    <h6 class="mt-3">Events</h6>
                                    <insite:TextBox runat="server" ID="OutputAudioClientSideEvents" ReadOnly="true" TextMode="MultiLine" Rows="10" />
                                </td>
                            </tr>
                        </table>

                        <script type="text/javascript">
                            (function () {
                                const element = <%= OutputAudioClientSide.ClientID %>;
                                const elOutput =  <%= OutputAudioClientSideOutput.ClientID %>;
                                const elEvents =  <%= OutputAudioClientSideEvents.ClientID %>;

                                $(element).on(inSite.common.outputAudio.event.delete, function (e) {
                                    e.preventDefault();
                                });

                                window.outputAudioTest = {
                                    loadData: function () { element.outputAudio.loadData(); },
                                    play: function () { element.outputAudio.play(); },
                                    pause: function () { element.outputAudio.pause(); },
                                    stop: function () { element.outputAudio.stop(); },
                                    disable: function () { element.outputAudio.disable(); },
                                    enable: function () { element.outputAudio.enable(); },
                                    delete: function () { element.outputAudio.delete(); },
                                    mute: function () { element.outputAudio.mute(); },
                                    unmute: function () { element.outputAudio.unmute(); },
                                    show: function () { element.outputAudio.show(); },
                                    hide: function () { element.outputAudio.hide(); },

                                    state: function () { output('state'); },
                                    volume: function () { output('volume'); },
                                    setVolume: function () { element.outputAudio.volume = Math.random(); },
                                    position: function () { output('position'); },
                                    duration: function () { output('duration'); },
                                    currentTime: function () { output('currentTime'); },
                                    attemptLimit: function () { output('attemptLimit'); },
                                    setAttemptLimit: function () { element.outputAudio.attemptLimit = Math.random() * 10; },
                                    attemptNow: function () { output('attemptNow'); },
                                    setAttemptNow: function () { element.outputAudio.attemptNow = Math.random() * 10; },
                                    isEnabled: function () { output('isEnabled'); },
                                    isMuted: function () { output('isMuted'); },
                                };

                                {
                                    let allEvents = '';
                                    for (var en in inSite.common.outputAudio.event) {
                                        if (inSite.common.outputAudio.event.hasOwnProperty(en)) {
                                            if (allEvents.length > 0)
                                                allEvents += ' ';

                                            allEvents += inSite.common.outputAudio.event[en];
                                        }
                                    }

                                    $(element)
                                        .on(allEvents, function (e) {
                                            elEvents.value = e.type + '\r\n' + elEvents.value;
                                        });
                                }

                                function output(name) {
                                    var value = element.outputAudio[name];
                                    if (value instanceof Blob)
                                        value = 'Blob { size: ' + String(value.size) + ', type: ' + String(value.type) + ' }';
                                    elOutput.value = name + ' = ' + String(value);
                                }
                            })();
                        </script>

                        <h3 class="mt-3">Validation</h3>

                        <div class="row">
                            <div class="col-md-6">
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Audio
                                    </label>
                                    <insite:OutputAudio runat="server" ID="OutputAudioValidation_Audio" AudioURL="/UI/Admin/Foundations/AudioSample.mp3" CausesValidation="true" ValidationGroup="OutputAudioValidation" AllowDelete="true" />
                                </div>
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Required Text
                                        <insite:RequiredValidator runat="server" ControlToValidate="OutputAudioValidation_Text" FieldName="Required Text" ValidationGroup="OutputAudioValidation" />
                                    </label>
                                    <insite:TextBox runat="server" ID="OutputAudioValidation_Text" />
                                </div>
                            </div>
                        </div>
                        
                    </section>
                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" Title="Input (Date/Time)">

            <h2>Date Picker</h2>

            <p>
                A lightweight and powerful date / time <a href="<%= AroundHomeUrl %>/components/date-picker.html">picker</a> component.
            </p>

            <pre><code>
    &lt;insite:DateSelector runat="server" /&gt;
    &lt;insite:DateTimeOffsetSelector runat="server" /&gt;
                    </code></pre>

            <h3 class="mt-3">Examples</h3>

            <div class="row">
                <div class="col-md-6">
                    <insite:DateSelector runat="server" ID="DateSelector" />
                    Date Picker<br />
                    <br />
                    <insite:DateTimeOffsetSelector runat="server" ID="DateTimeOffsetSelector" />
                    DateTime Picker
                </div>
            </div>

        </insite:NavItem>

        <insite:NavItem runat="server" Title="Input (HTML/Markdown)">


            <h2>Editors</h2>

            <h3>Markdown</h3>
            <div class="row">
                <div class="col-lg-12">
                    <div style="position: relative;">
                        <insite:MarkdownEditor runat="server" ID="MarkdownEditor" UploadControl="MarkdownUpload" TranslationControl="MarkdownTranslation" />
                        <div style="margin-top: 3px;">
                            <insite:EditorTranslation runat="server" ID="MarkdownTranslation" EnableMarkdownConverter="true" TableContainerID="MarkdownTranslationTableContainer" />
                        </div>
                        <insite:EditorUpload runat="server" ID="MarkdownUpload" />
                        <div runat="server" id="MarkdownTranslationTableContainer"></div>
                    </div>
                </div>
            </div>

            <h3 class="mt-3">HTML</h3>
            <div class="row">
                <div class="col-lg-12">
                    <div style="position: relative;">
                        <insite:HtmlEditor runat="server" ID="HtmlEditor" UploadControl="HtmlUpload" TranslationControl="HtmlTranslation" />
                        <div style="margin-top: 3px;">
                            <insite:EditorTranslation runat="server" ID="HtmlTranslation" TableContainerID="HtmlTranslationTableContainer" />
                        </div>
                        <insite:EditorUpload runat="server" ID="HtmlUpload" />
                        <div runat="server" id="HtmlTranslationTableContainer"></div>
                    </div>
                </div>
            </div>

        </insite:NavItem>

        <insite:NavItem runat="server" Title="Modal">

            <h2>Modal Finders</h2>

            <pre><code>
&lt;insite:FindAchievement runat="server" MaxSelectionCount="0" /&gt;
&lt;insite:FindDepartment runat="server" MaxSelectionCount="0" /&gt;
&lt;insite:FindGroup runat="server" MaxSelectionCount="0" /&gt;
&lt;insite:FindWorkflowForm runat="server" MaxSelectionCount="0" /&gt;
&lt;insite:FindOrganization runat="server" MaxSelectionCount="0" /&gt;
&lt;insite:FindUser runat="server" MaxSelectionCount="0" /&gt;
&lt;insite:FindBank runat="server" MaxSelectionCount="0" /&gt;
&lt;insite:FindBankForm runat="server" MaxSelectionCount="0" /&gt;
&lt;insite:FindBankOccupation runat="server" MaxSelectionCount="0" /&gt;
&lt;insite:FindBankFramework runat="server" MaxSelectionCount="0" /&gt;
&lt;insite:FindEvent runat="server" MaxSelectionCount="0" /&gt;
                </code></pre>

            <br />
            <div class="row">
                <div class="col-md-4">
                    <insite:FindAchievement runat="server" ID="FindAchievement1" MaxSelectionCount="0" />
                    Find multiple achievements<br />
                    <br />
                    <insite:FindDepartment runat="server" ID="FindDepartment1" MaxSelectionCount="0" />
                    Find multiple departments<br />
                    <br />
                    <insite:FindGroup runat="server" ID="FindGroup1" MaxSelectionCount="0" />
                    Find multiple groups<br />
                    <br />
                    <insite:FindWorkflowForm runat="server" ID="FindSurvey1" MaxSelectionCount="0" />
                    Find multiple forms<br />
                    <br />
                    <insite:FindOrganization runat="server" ID="FindOrganization1" MaxSelectionCount="0" />
                    Find multiple organizations<br />
                    <br />
                    <insite:FindUser runat="server" ID="FindUser1" MaxSelectionCount="0" />
                    Find multiple users<br />
                    <br />
                    <insite:FindBank runat="server" ID="FindBank1" MaxSelectionCount="0" />
                    Find multiple banks<br />
                    <br />
                    <insite:FindBankForm runat="server" ID="FindBankForm1" MaxSelectionCount="0" />
                    Find multiple bank forms<br />
                    <br />
                    <insite:FindBankOccupation runat="server" ID="FindBankOccupation1" MaxSelectionCount="0" />
                    Find multiple bank occupations<br />
                    <br />
                    <insite:FindBankFramework runat="server" ID="FindBankFramework1" MaxSelectionCount="0" />
                    Find multiple bank frameworks<br />
                    <br />
                    <insite:FindEvent runat="server" ID="FindEvent1" MaxSelectionCount="0" />
                    Find multiple events<br />
                    <br />
                    <insite:FindPerson runat="server" ID="FindPerson1" MaxSelectionCount="0" />
                    Find multiple persons<br />
                    <br />
                    <insite:FindPermission runat="server" ID="FindPermission1" MaxSelectionCount="0" />
                    Find multiple permissions<br />
                    <br />
                    <insite:FindPeriod runat="server" ID="FindPeriod1" MaxSelectionCount="0" />
                    Find multiple periods<br />
                    <br />
                </div>
                <div class="col-md-4">
                    <insite:FindAchievement runat="server" ID="FindAchievement2" />
                    Find one achievement only<br />
                    <br />
                    <insite:FindDepartment runat="server" ID="FindDepartment2" />
                    Find one department only<br />
                    <br />
                    <insite:FindGroup runat="server" ID="FindGroup2" />
                    Find one group only<br />
                    <br />
                    <insite:FindWorkflowForm runat="server" ID="FindSurvey2" />
                    Find one form only<br />
                    <br />
                    <insite:FindOrganization runat="server" ID="FindOrganization2" />
                    Find one organization only<br />
                    <br />
                    <insite:FindUser runat="server" ID="FindUser2" />
                    Find one user only<br />
                    <br />
                    <insite:FindBank runat="server" ID="FindBank2" />
                    Find one bank only<br />
                    <br />
                    <insite:FindBankForm runat="server" ID="FindBankForm2" />
                    Find one bank form only<br />
                    <br />
                    <insite:FindBankOccupation runat="server" ID="FindBankOccupation2" />
                    Find one bank occupation only<br />
                    <br />
                    <insite:FindBankFramework runat="server" ID="FindBankFramework2" />
                    Find one bank framework only<br />
                    <br />
                    <insite:FindEvent runat="server" ID="FindEvent2" />
                    Find one event only<br />
                    <br />
                    <insite:FindPerson runat="server" ID="FindPerson2" />
                    Find one person only<br />
                    <br />
                    <insite:FindPermission runat="server" ID="FindPermission2" />
                    Find one permission only<br />
                    <br />
                    <insite:FindPeriod runat="server" ID="FindPeriod2" />
                    Find one period only<br />
                    <br />
                </div>
                <div class="col-md-4">
                    <insite:FindAchievement runat="server" ID="FindAchievement3" MaxSelectionCount="3" />
                    Find at most 3 achievements<br />
                    <br />
                    <insite:FindDepartment runat="server" ID="FindDepartment3" MaxSelectionCount="3" />
                    Find at most 3 departments<br />
                    <br />
                    <insite:FindGroup runat="server" ID="FindGroup3" MaxSelectionCount="3" />
                    Find at most 3 groups<br />
                    <br />
                    <insite:FindWorkflowForm runat="server" ID="FindSurvey3" MaxSelectionCount="3" />
                    Find at most 3 forms<br />
                    <br />
                    <insite:FindOrganization runat="server" ID="FindOrganization3" MaxSelectionCount="3" />
                    Find at most 3 organizations<br />
                    <br />
                    <insite:FindUser runat="server" ID="FindUser3" MaxSelectionCount="3" />
                    Find at most 3 users<br />
                    <br />
                    <insite:FindBank runat="server" ID="FindBank3" MaxSelectionCount="3" />
                    Find at most 3 banks<br />
                    <br />
                    <insite:FindBankForm runat="server" ID="FindBankForm3" MaxSelectionCount="3" />
                    Find at most 3 bank forms<br />
                    <br />
                    <insite:FindBankOccupation runat="server" ID="FindBankOccupation3" MaxSelectionCount="3" />
                    Find at most 3 bank occupations<br />
                    <br />
                    <insite:FindBankFramework runat="server" ID="FindBankFramework3" MaxSelectionCount="3" />
                    Find at most 3 bank frameworks<br />
                    <br />
                    <insite:FindEvent runat="server" ID="FindEvent3" MaxSelectionCount="3" />
                    Find at most 3 events<br />
                    <br />
                    <insite:FindPerson runat="server" ID="FindPerson3" MaxSelectionCount="3" />
                    Find at most 3 persons<br />
                    <br />
                    <insite:FindPermission runat="server" ID="FindPermission3" MaxSelectionCount="3" />
                    Find at most 3 permissions<br />
                    <br />
                    <insite:FindPeriod runat="server" ID="FindPeriod3" MaxSelectionCount="3" />
                    Find at most 3 periods<br />
                    <br />
                </div>
            </div>

        </insite:NavItem>

        <insite:NavItem runat="server" Title="Navigation">

            <h2>Navs</h2>

            <p>
                Use <a href="<%= AroundHomeUrl %>/components/tabs.html">tabs</a> or <a href="<%= AroundHomeUrl %>/components/pills.html">pills</a> to organize content when you need 
                    to stack a list of panels horizontally or vertically. Icons here are optional.
            </p>

            <pre><code>
&lt;insite:Nav runat="server" ItemAlignment="Horizontal|Vertical" ItemType="Tabs|Pills"&gt;
    &lt;insite:NavItem runat="server" Icon="fas fa-home" Title="Home"&gt;
        Home Panel Content
    &lt;/insite:NavItem&gt;
    &lt;insite:NavItem runat="server" Icon="fas fa-user" Title="Profile"&gt;
        Profile Panel Content
    &lt;/insite:NavItem&gt;
    &lt;insite:NavItem runat="server" Title="Settings"&gt;
        Settings Panel Content
    &lt;/insite:NavItem&gt;
&lt;/insite:Nav&gt;
                </code></pre>

            <h3 class="mt-3">Horizontal Example</h3>

            <h6>Tabs</h6>

            <insite:Nav runat="server" ItemAlignment="Horizontal" ItemType="Tabs">
                <insite:NavItem runat="server" Icon="fas fa-home" Title="Home">
                    Home Panel Content
                </insite:NavItem>
                <insite:NavItem runat="server" Icon="fas fa-user" Title="Profile">
                    Profile Panel Content
                </insite:NavItem>
                <insite:NavItem runat="server" Title="Settings">
                    Settings Panel Content
                </insite:NavItem>
            </insite:Nav>
            <br />

            <h6>Pills</h6>

            <insite:Nav runat="server" ItemAlignment="Horizontal" ItemType="Pills">
                <insite:NavItem runat="server" Icon="fas fa-home" Title="Home">
                    Home Panel Content
                </insite:NavItem>
                <insite:NavItem runat="server" Icon="fas fa-user" Title="Profile">
                    Profile Panel Content
                </insite:NavItem>
                <insite:NavItem runat="server" Title="Settings">
                    Settings Panel Content
                </insite:NavItem>
            </insite:Nav>
            <br />

            <h3 class="mt-3">Vertical Example</h3>

            <h6>Tabs</h6>

            <insite:Nav runat="server" ItemAlignment="Vertical" ItemType="Tabs">
                <insite:NavItem runat="server" Icon="fas fa-home" Title="Home">
                    Home Panel Content
                </insite:NavItem>
                <insite:NavItem runat="server" Icon="fas fa-user" Title="Profile">
                    Profile Panel Content
                </insite:NavItem>
                <insite:NavItem runat="server" Title="Settings">
                    Settings Panel Content
                </insite:NavItem>
            </insite:Nav>
            <br />

            <h6>Pills</h6>

            <insite:Nav runat="server" ItemAlignment="Vertical" ItemType="Pills">
                <insite:NavItem runat="server" Icon="fas fa-home" Title="Home">
                    Home Panel Content
                </insite:NavItem>
                <insite:NavItem runat="server" Icon="fas fa-user" Title="Profile">
                    Profile Panel Content
                </insite:NavItem>
                <insite:NavItem runat="server" Title="Settings">
                    Settings Panel Content
                </insite:NavItem>
            </insite:Nav>

        </insite:NavItem>

        <insite:NavItem runat="server" Title="Table">

            <h2>Tables</h2>

            <p>
                <a href="<%= AroundHomeUrl %>/components/tables.html">Tables</a> should be response and striped.
    Use a Repeater control for maximum simplicity and flexibility.
    Whenever possible, avoid using heavyweight UI controls such as Microsoft GridView.
            </p>

            <h3 class="mt-3">Example</h3>

            <asp:Repeater runat="server" ID="TableExample">
                <HeaderTemplate>
                    <div class="table-responsive">
                        <table class="table table-striped table-hover">
                            <thead>
                                <tr>
                                    <th class="text-end">#</th>
                                    <th>First Name</th>
                                    <th>Last Name</th>
                                    <th>Position</th>
                                    <th>Phone</th>
                                </tr>
                            </thead>
                            <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="<%# Eval("RowClass") %>">
                        <th scope="row" class="text-end"><%# Eval("Sequence") %></th>
                        <td><%# Eval("FirstName") %></td>
                        <td><%# Eval("LastName") %></td>
                        <td><%# Eval("Email") %></td>
                        <td><%# Eval("Phone") %></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody>
        </table>
        </div>
                </FooterTemplate>
            </asp:Repeater>

        </insite:NavItem>

        <insite:NavItem runat="server" Title="Toast">

            <h2>Toasts</h2>

            <p>
                <a href="<%= AroundHomeUrl %>/components/toasts.html">Toasts</a> should always include an icon.
            </p>

            <pre><code>
    &lt;insite:Toast runat="server" Icon="info" Title="Information" Text="This is an information toast." Indicator="Information" /&gt;
    &lt;insite:Toast runat="server" Icon="check-circle" Title="Success" Text="Here is an example of a toast displayed after some successful operation." Indicator="Success" /&gt;
                    </code></pre>

            <h3 class="mt-3">Examples</h3>

            <insite:Toast runat="server" Icon="info" Title="Information" Text="Information toast." Indicator="Information" />
            <insite:Toast runat="server" Icon="check-circle" Title="Success" Text="Success toast." Indicator="Success" />
            <insite:Toast runat="server" Icon="slash" Title="Error" Text="Error toast." Indicator="Error" />
            <insite:Toast runat="server" Icon="alert-triangle" Title="Warning" Text="Warning toast." Indicator="Warning" />
            <insite:Toast runat="server" Text="Default toast." />

        </insite:NavItem>

        <insite:NavItem runat="server" Title="Validation">

            <h2>Validation</h2>

            <pre><code>
&lt;insite:InSiteValidationSummary runat="server" /&gt;
                </code></pre>

            <h3 class="mt-3">Examples</h3>

            <insite:ValidationSummary runat="server" ID="ValidationSummary" ValidationGroup="Validation" />

            <div class="row">
                <div class="col-md-6">

                    <div class="form-group mb-3">
                        <label class="form-label" for="<%# ValidationField.ClientID %>">
                            <insite:Literal runat="server" Text="Required Field" />
                            <insite:RequiredValidator runat="server" ControlToValidate="ValidationField" FieldName="Required Field" ValidationGroup="Validation" />
                        </label>
                        <div>
                            <insite:TextBox runat="server" ID="ValidationField" EmptyMessage="Required Field" />
                        </div>
                    </div>

                </div>
            </div>

            <div class="row">
                <div class="col-lg-12">
                    <insite:Button runat="server" ID="ValidateButton" Text="Validate" ButtonStyle="Default" ValidationGroup="Validation" />
                </div>
            </div>

        </insite:NavItem>

        <insite:NavItem runat="server" Title="Page">

            <b>Dashboard pages:</b>

            <insite:Accordion runat="server">
                <insite:AccordionPanel runat="server" Icon="far fa-abacus" Title="Counters">
                    Counters -> abacus
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-database" Title="Administration Tools">
                    Administration Tools -> tools
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-history" Title="Recent Changes">
                    Recent Changes (History) -> history
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-file-chart-line" Title="Summaries">
                    Summaries, Results ->  file-chart-line
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-signal" Title="Counts">
                    Counts ->  signal
                </insite:AccordionPanel>
            </insite:Accordion>

            <b>Search pages:</b>
            <insite:Accordion runat="server">
                <insite:AccordionPanel runat="server" Icon="far fa-search" Title="Search">
                    Search -> search
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-database" Title="Results">
                    Results -> database
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-mail-bulk" Title="Build Mailing List">
                    Build Mailing List -> mail-bulk
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-camera" Title="Snapshots">
                    Snapshots -> camera
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-download" Title="Downloads">
                    Downloads -> download
                </insite:AccordionPanel>
            </insite:Accordion>

            <b>Common</b>
            <insite:Accordion runat="server">
                <insite:AccordionPanel runat="server" Icon="far fa-sitemap" Title="Outline">
                    Outline, Sitemap -> sitemap
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-sitemap" Title="Connections">
                    People (connections), Relationships  -> sitemap
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-level-up-alt" Title="Upstream Relationships">
                    Upstream Relationships ->  level-up-alt
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-level-down-alt" Title="Downstream Relationships">
                    Downstream Relationships  -> level-down-alt 
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-chart-bar" Title="Report">
                    Report -> chart-bar
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-tv" Title="Wallpapers">
                    Wallpapers-> tv
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-search" Title="Review">
                    Review-> tv
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-external-link" Title="References">
                    References -> external-link
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-edit" Title="Content">
                    Content -> edit 
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-tag" Title="Category">
                    Category -> tag 
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-upload" Title="File">
                    File (Upload) -> upload
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-cogs" Title="Fields">
                    Fields (for Upload) -> cogs
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-cogs" Title="Configuration">
                    Configuration ->  cogs
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-cogs" Title="How to">
                    How to ->  cogs
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-paperclip" Title="Attachment">
                    Attachment, Attached File -> paperclip
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-images" Title="Images">
                    Images -> images
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-print" Title="Print">
                    Print -> print
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-exclamation-circle" Title="Problem Information">
                    Problem Information -> exclamation-circle
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-exclamation-triangle" Title="Duplicates">
                    Duplicates -> exclamation-triangle
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-check-circle" Title="Status">
                    Status -> check-circle
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-file-check" Title="Confirm">
                    Confirm -> file-check
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-comments" Title="Comments">
                    Comments -> comments
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-comment" Title="Comment">
                    Comment -> comment
                </insite:AccordionPanel>
            </insite:Accordion>

            <b>Achievements</b>
            <insite:Accordion runat="server">
                <insite:AccordionPanel runat="server" Icon="far fa-trophy" Title="Achievements">
                    Achievements -> trophy
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-award" Title="Credentials">
                    Credentials -> award
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-file-certificate" Title="Certificate">
                    Certificate -> file-certificate
                </insite:AccordionPanel>
            </insite:Accordion>

            <b>Assessments</b>
            <insite:Accordion runat="server">
                <insite:AccordionPanel runat="server" Icon="far fa-balance-scale" Title="Assessments">
                    Assessments, Banks -> balance-scale
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-check-square" Title="Quiz">
                    Quiz -> check-square
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-window" Title="Form">
                    Form -> window
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-th-list" Title="Section, Set">
                    Section, Set -> th-list
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-clipboard-list" Title="Specifications">
                    Specifications -> clipboard-list
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-filter" Title="Criteria">
                    Criteria -> filter
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-books" Title="Rationale">
                    Rationale -> books
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-globe" Title="Safe Exam Browser">
                    Safe Exam Browser -> globe
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-tasks" Title="Attempts">
                    Attempts -> tasks
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-chart-bar" Title="Time Series">
                    Time Series -> chart-bar
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-tachometer-alt" Title="Statistics">
                    Statistics -> tachometer-alt
                </insite:AccordionPanel>
            </insite:Accordion>

            <b>Contacts</b>
            <insite:Accordion runat="server">
                <insite:AccordionPanel runat="server" Icon="far fa-user" Title="Contact">
                    Contact, Person, User, Participant, People, Account -> user
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-users" Title="Group">
                    Contacts, Group -> users
                </insite:AccordionPanel>

                <insite:AccordionPanel runat="server" Icon="far fa-city" Title="Organizations">
                    Organizations -> city
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-mailbox" Title="Senders">
                    Senders -> mailbox
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-building" Title="Departments">
                    Departments -> building
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-industry" Title="Divisions">
                    Divisions -> industry
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-user-tie" Title="Employer">
                    Employer, Company, Customers -> user-tie
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-id-card" Title="Membership">
                    Membership -> id-card
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-home" Title="Address">
                    Address (es) -> home
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-medal" Title="Experiences">
                    Experiences -> medal
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-key" Title="Permissions">
                    Permissions, Resource Permissions -> key
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-key" Title="Password">
                    Password -> key
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-user-cog" Title="Department/Profile/Person Settings">
                    Department/Profile/Person Settings -> user-cog 
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-sliders-h" Title="Role Settings">
                    Role Settings -> sliders-h
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-clipboard-check" Title="Standard Validations/Permissions">
                    Standard Validations/Permissions -> clipboard-check
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-sign-in-alt" Title="Authentications">
                    Authentications -> sign-in-alt
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-user-secret" Title="Impersonations">
                    Impersonations, Authorization -> user-secret
                </insite:AccordionPanel>
            </insite:Accordion>

            <b>Courses</b>
            <insite:Accordion runat="server">
                <insite:AccordionPanel runat="server" Icon="far fa-users-class" Title="Courses">
                    Events -> users-class
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-file" Title="Course Page">
                    Course Page -> file
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-car" Title="Test Drive Commands">
                    Test Drive Commands -> car
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-walking" Title="Progressions">
                    Progressions -> walking
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-chalkboard-teacher" Title="Lesson">
                    Lesson -> chalkboard-teacher
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-plug" Title="Lti Link">
                    Lti Link -> plug
                </insite:AccordionPanel>
            </insite:Accordion>

            <b>Events</b>
            <insite:Accordion runat="server">
                <insite:AccordionPanel runat="server" Icon="far fa-calendar-alt" Title="Events">
                    Events, Class, Exam -> calendar-alt
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-assistive-listening-systems" Title="Accommodations">
                    Accommodations -> assistive-listening-systems
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-graduation-cap" Title="School/Training Provider">
                    School/Training Provider -> graduation-cap
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-alarm-clock" Title="Timers">
                    Timers -> alarm-clock
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-money-check-alt" Title="Seats">
                    Seats, Payment -> money-check-alt
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-id-card" Title="Registrations">
                    Registrations -> id-card
                </insite:AccordionPanel>
            </insite:Accordion>

            <b>Gradebooks</b>
            <insite:Accordion runat="server">
                <insite:AccordionPanel runat="server" Icon="far fa-spell-check" Title="Gradebooks">
                    Gradebooks -> spell-check
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-ballot-check" Title="Scores">
                    Scores -> ballot-check
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="fal fa-tasks" Title="Learning Mastery">
                    Learning Mastery, Outcomes -> fal fa-tasks
                </insite:AccordionPanel>
            </insite:Accordion>

            <b>Invoice</b>
            <insite:Accordion runat="server">
                <insite:AccordionPanel runat="server" Icon="far fa-file-invoice-dollar" Title="Invoice">
                    Invoice -> file-invoice-dollar
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-list" Title="Items">
                    Items -> list
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-credit-card-front" Title="Payments">
                    Payments -> credit-card-front
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-pallet" Title="Product">
                    Product -> pallet 
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-badge-percent" Title="Discounts">
                    Discounts -> badge-percent
                </insite:AccordionPanel>
            </insite:Accordion>

            <b>Cases</b>
            <insite:Accordion runat="server">
                <insite:AccordionPanel runat="server" Icon="far fa-exclamation" Title="Case">
                    Case -> exclamation
                </insite:AccordionPanel>
            </insite:Accordion>

            <b>Messages</b>
            <insite:Accordion runat="server">
                <insite:AccordionPanel runat="server" Icon="far fa-paper-plane" Title="Message">
                    Message -> paper-plane
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-link" Title="Links">
                    Links -> link
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-users" Title="Message Subscribers">
                    Message Subscribers -> users
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-calendar-alt" Title="Schedule Mailout">
                    Schedule Mailout -> calendar-alt
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-mail-bulk" Title="Message Deliveries">
                    Message Deliveries, Mailouts -> mail-bulk
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-mouse-pointer" Title="Clicks">
                    Clicks -> mouse-pointer
                </insite:AccordionPanel>
            </insite:Accordion>

            <b>Standards</b>
            <insite:Accordion runat="server">
                <insite:AccordionPanel runat="server" Icon="far fa-ruler-triangle" Title="Standards">
                    Standards -> ruler-triangle
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-ruler-triangle" Title="Competencies">
                    Competencies, Profile (CMDS) -> ruler-triangle
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-id-badge" Title="Profile">
                    Profile -> id-badge
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-box-open" Title="Collection">
                    Collection-> box-open
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-file-alt" Title="Document">
                    Document-> file-alt
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-balance-scale" Title="Validations">
                    Validations, Status (of Validations)-> balance-scale
                </insite:AccordionPanel>
            </insite:Accordion>

            <b>Forms</b>
            <insite:Accordion runat="server">
                <insite:AccordionPanel runat="server" Icon="far fa-check-square" Title="Forms">
                    Forms -> check-square
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-question" Title="Questions">
                    Questions -> question
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-list" Title="Options">
                    Options -> list
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-table" Title="Define Likert Scales">
                    Define Likert Scales -> table
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-poll-people" Title="Form Submissions">
                    Form Submissions, Voter responses (registrations)  -> poll-people
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-ballot-check" Title="Answers">
                    Answers  -> ballot-check
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-clipboard-list" Title="Frequency Distribution Analysis, Correlation Analysis">
                    Frequency Distribution Analysis, Correlation Analysis -> clipboard-list
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-chart-bar" Title="Time Series Analysis">
                    Time Series Analysis-> chart-bar
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-clone" Title="Multiple Submissions per Form Respondent">
                    Multiple Submissions per Form Respondent -> clone
                </insite:AccordionPanel>
            </insite:Accordion>

            <b>Sites</b>
            <insite:Accordion runat="server">
                <insite:AccordionPanel runat="server" Icon="far fa-cloud" Title="Site">
                    Site -> cloud
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-file" Title="Page">
                    Page -> file
                </insite:AccordionPanel>
            </insite:Accordion>

            <b>Votes</b>
            <insite:Accordion runat="server">
                <insite:AccordionPanel runat="server" Icon="far fa-vote-yea" Title="Votes">
                    Election (Votes) -> vote-yea
                </insite:AccordionPanel>
            </insite:Accordion>

            <b>CMDS:</b>
            <insite:Accordion runat="server">
                <insite:AccordionPanel runat="server" Icon="far fa-map-marked-alt" Title="Training Program">
                    Training Program -> map-marked-alt
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-lightbulb-on" Title="Knowledge and Skills">
                    Knowledge and Skills -> lightbulb-on
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-industry" Title="Division">
                    Division -> industry
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-building" Title="Department">
                    Department -> building
                </insite:AccordionPanel>
            </insite:Accordion>

            <b>IECBC:</b>
            <insite:Accordion runat="server">
                <insite:AccordionPanel runat="server" Icon="far fa-list" Title="Job Application">
                    Job Application -> list 
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-address-card" Title="Candidate">
                    Candidate -> address-card
                </insite:AccordionPanel>
            </insite:Accordion>

            <b>SkilledTradesBC:</b>
            <insite:Accordion runat="server">
                <insite:AccordionPanel runat="server" Icon="far fa-file-medical-alt" Title="Distribution Status">
                    Distribution Status -> file-medical-alt
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-box-full" Title="Distribution Packages">
                    Distribution Packages -> box-full
                </insite:AccordionPanel>
            </insite:Accordion>

            <b>Admin</b>
            <insite:Accordion runat="server">
                <insite:AccordionPanel runat="server" Icon="far fa-question-circle" Title="Help">
                    Help -> question-circle
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-scroll" Title="Terms">
                    Terms -> scroll
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-envelope" Title="Support">
                    Support -> envelope
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-chart-line" Title="Key Performance Indicators">
                    Key Performance Indicators -> chart-line
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-debug" Title="Tests">
                    Tests -> debug
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-engine-warning" Title="Alerts">
                    Alerts ->  engine-warning
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-bolt" Title="Triggers">
                    Triggers ->  bolt
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-plug" Title="API Requests">
                    API Requests -> plug
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-inbox-in" Title="Input">
                    Input -> inbox-in
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-inbox-out" Title="Output">
                    Output -> inbox-out
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-user-cog" Title="Developer">
                    Developer -> user-cog
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-gift" Title="Holiday">
                    Holiday -> gift
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-cog" Title="Aggregate">
                    Aggregate -> cog
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-list-ul" Title="Changes">
                    Changes -> list-ul
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-terminal" Title="Commands">
                    Commands -> terminal
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-map" Title="Tombstone">
                    Tombstone -> map
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-tag" Title="UI Label(s)">
                    UI Label(s) ->  tag 
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-album-collection" Title="Collections">
                    Collections ->  album-collection
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-album" Title="Collection Item">
                    Collection Item ->  album 
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-tasks" Title="Process">
                    Process ->  tasks
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-toolbox" Title="Toolkits">
                    Toolkits -> toolbox
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-location" Title="Action">
                    Action -> location
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-book" Title="Description">
                    Description ->  book
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-table" Title="Table">
                    Table -> table
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-columns" Title="Column">
                    Column -> columns
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-grip-lines" Title="Constraint">
                    Constraint ->  grip-lines
                </insite:AccordionPanel>
                <insite:AccordionPanel runat="server" Icon="far fa-filter" Title="Precondition">
                    Precondition ->  filter
                </insite:AccordionPanel>
            </insite:Accordion>
        </insite:NavItem>

    </insite:Nav>

</asp:Content>