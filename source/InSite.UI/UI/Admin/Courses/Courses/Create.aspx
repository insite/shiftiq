<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Courses.Courses.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Course" />

    <section runat="server" ID="CoursePanel" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-chalkboard-teacher me-1"></i>
            Course
        </h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <div class="row">
                    <div class="col-md-6">

                        <div class="form-group mb-3">
                            <label class="form-label">Course-Creation Tool</label>
                            <div>
                                <insite:CreationTypeComboBox runat="server" ID="CreationType" TypeName="Course" />
                            </div>
                            <div class="form-text">What do you want to create?</div>
                        </div>

                        <asp:MultiView runat="server" ID="CreateMultiView" ActiveViewIndex="0">
                            <asp:View runat="server" ID="CreateOneView">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Course Name
                                        <insite:RequiredValidator runat="server" ID="CourseNameValidator" ControlToValidate="CourseName" ValidationGroup="Course" />
                                    </label>
                                    <div>
                                        <insite:TextBox runat="server" ID="CourseName" Text="New Course" MaxLength="200" />
                                    </div>
                                </div>

                            </asp:View>
                            <asp:View runat="server" ID="CreateUploadView">

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        File Type
                                    </label>
                                    <div>
                                        <insite:ComboBox runat="server" ID="UploadFileType" Width="100%">
                                            <Items>
                                                <insite:ComboBoxOption Text="Markdown" Value="MD" />
                                                <insite:ComboBoxOption Text="JSON" Value="JSON" Selected="true" />
                                            </Items>
                                        </insite:ComboBox>
                                    </div>
                                </div>

                                <insite:FileUploadV1 runat="server" ID="CreateUploadFile" LabelText="Select and Upload JSON File" FileUploadType="Unlimited" />

                            </asp:View>
                        </asp:MultiView>

                    </div>
                </div>

            </div>
        </div>

    </section>

    <div class="row">
        <div class="col-md-12">

            <insite:SaveButton runat="server" ID="CreateButton" ValidationGroup="Course" />
            <insite:CancelButton runat="server" ID="CancelButton" />

        </div>
    </div>

</asp:Content>
