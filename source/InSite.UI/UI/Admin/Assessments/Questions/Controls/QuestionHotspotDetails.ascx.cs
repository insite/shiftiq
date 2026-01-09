using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Admin.Assessments.Attachments.Utilities;
using InSite.Admin.Assessments.Options.Controls;
using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.Infrastructure;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Assessments.Questions.Controls
{
    public partial class QuestionHotspotDetails : OptionWriteRepeater
    {
        #region Classes

        [Serializable]
        private class OptionCollection : ItemCollection
        {
            public OptionCollection(Guid questionId)
                : base(questionId)
            {

            }
        }

        [Serializable]
        private class OptionItem : ItemCollection.DataItem
        {
            public Guid Identifier { get; }
            public HotspotShape Shape { get; private set; }
            public string ShapeIcon { get; private set; }
            public decimal Points { get; set; }

            public OptionItem(HotspotShape shape)
                : base()
            {
                Identifier = UniqueIdentifier.Create();

                SetShape(shape);
            }

            public OptionItem(HotspotOption option)
                : base(option.Number)
            {
                Identifier = option.Identifier;

                SetShape(option.Shape);

                Text.Set(option.Content.Title);
                Points = option.Points;
            }

            private void SetShape(HotspotShape shape)
            {
                Shape = shape.Clone();

                if (shape is HotspotShapeRectangle)
                    ShapeIcon = "rectangle";
                else if (shape is HotspotShapeCircle)
                    ShapeIcon = "circle";
            }

            public override Option GetOption() => throw new NotImplementedException();

            public HotspotOption GetHotspotOption()
            {
                var option = new HotspotOption(Identifier, Shape);
                option.Number = Number;
                option.Content.Title.Set(Text);
                option.Points = Points;
                return option;
            }
        }

        [Serializable, JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        private class ImageData
        {
            public string Path { get; private set; }

            [JsonProperty(PropertyName = "src")]
            public string Source
            {
                get
                {
                    if (Path.StartsWith("/"))
                        return Path;

                    var data = File.ReadAllBytes(Path);
                    var mime = MimeMapping.GetContentType(Path);

                    return $"data:{mime};base64,{Convert.ToBase64String(data)}";
                }
            }

            [JsonProperty(PropertyName = "width")]
            public int Width { get; private set; }

            [JsonProperty(PropertyName = "height")]
            public int Height { get; private set; }

            [JsonConstructor]
            private ImageData()
            {

            }

            public static ImageData Create(string path)
            {
                if (!File.Exists(path))
                    return null;

                AttachmentImage imgProps;
                using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    imgProps = AttachmentHelper.ReadImageProps(stream);

                if (imgProps?.Actual.HasValue != true)
                    return null;

                return new ImageData
                {
                    Path = path,
                    Width = imgProps.Actual.Width,
                    Height = imgProps.Actual.Height
                };
            }

            public static ImageData Create(string url, int width, int height)
            {
                return new ImageData
                {
                    Path = url,
                    Width = width,
                    Height = height
                };
            }

            public static ImageData Create(HotspotImage data) =>
                Create(data.Url, data.Width, data.Height);
        }

        [Serializable]
        private class ControlData
        {
            public ImageData ConfirmImage { get; set; }
            public ImageData NewImage { get; set; }
            public ImageData ExistImage { get; set; }
        }

        #endregion

        #region Properties

        private Guid? FrameworkIdentifier
        {
            get { return (Guid?)ViewState[nameof(FrameworkIdentifier)]; }
            set { ViewState[nameof(FrameworkIdentifier)] = value; }
        }

        private ControlData Data
        {
            get => (ControlData)ViewState[nameof(Data)];
            set => ViewState[nameof(Data)] = value;
        }

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ImageUploadedButton.Click += ImageUploadedButton_Click;
            UploadCancelButton.Click += UploadCancelButton_Click;

            UploadConfirmButton.Click += UploadConfirmButton_Click;
            UploadConfirmCancelButton.Click += UploadConfirmCancelButton_Click;

            ChangeImageButton.Click += ChangeImageButton_Click;

            AddRectangleOption.Click += AddRectangleOption_Click;
            AddCircleOption.Click += AddCircleOption_Click;

            Repeater.DataBinding += Repeater_DataBinding;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                ImageUpload.MaxFileSize = Organization.PlatformCustomization.UploadSettings.Images.MaximumFileSize;

                AddRectangleOption2.Attributes["data-actlike"] = AddRectangleOption.ClientID;
                AddCircleOption2.Attributes["data-actlike"] = AddCircleOption.ClientID;
            }
            else
            {
                GetShapes();
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            AddRectangleOption.Visible = !IsReadOnly;
            AddCircleOption.Visible = !IsReadOnly;

            base.OnPreRender(e);

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(SingleCorrectOptionRepeater),
                "init_reorder",
                $"optionWriteRepeater.initReorder('{ClientID}','{ItemsOrder.ClientID}');",
                true);

            if (!HttpRequestHelper.IsAjaxRequest)
                DrawImage();
        }

        protected override ItemCollection CreateItemCollection(Guid questionId)
        {
            if (questionId == Guid.Empty)
                return new OptionCollection(questionId);

            var question = ServiceLocator.BankSearch.GetQuestionData(questionId);
            FrameworkIdentifier = question.Set.Bank.Standard;
            return new OptionCollection(questionId);
        }

        protected override Repeater GetRepeater() => Repeater;

        protected override HiddenField GetItemsOrder() => ItemsOrder;

        protected override IEnumerable<BaseValidator> GetItemValidators(RepeaterItem item)
        {
            yield return (BaseValidator)item.FindControl("TextRequiredValidator");
        }

        #endregion

        #region Event handlers

        private void ImageUploadedButton_Click(object sender, EventArgs e)
        {
            try
            {
                Data.ConfirmImage = ImageData.Create(ImageUpload.FilePath);
            }
            catch (ApplicationError appex)
            {
                UploadStatus.AddMessage(AlertType.Error, appex.Message);
                return;
            }

            if (Data.ConfirmImage == null)
            {
                UploadStatus.AddMessage(AlertType.Error, "The uploaded image is corrupted.");
                return;
            }

            if (Data.ConfirmImage.Width < 200)
            {
                UploadStatus.AddMessage(AlertType.Error, "The minimum image width is 200 pixels.");
                return;
            }

            if (Data.ConfirmImage.Height < 200)
            {
                UploadStatus.AddMessage(AlertType.Error, "The minimum image height is 200 pixels.");
                return;
            }

            UploadConfirmImage.Src = Data.ConfirmImage.Source;

            MultiView.SetActiveView(UploadConfirmView);
        }

        private void UploadCancelButton_Click(object sender, EventArgs e)
        {
            MultiView.SetActiveView(ManageOptionsView);
        }

        private void UploadConfirmButton_Click(object sender, EventArgs e)
        {
            var oldImgData = GetImage();

            Data.NewImage = Data.ConfirmImage;
            Data.ConfirmImage = null;
            UploadConfirmImage.Src = null;

            MultiView.SetActiveView(ManageOptionsView);

            DrawImage();

            if (oldImgData != null && OptionItems.Count > 0)
            {
                var resizeArgs = new HotspotImage.ResizeEventArgs(
                    oldImgData.Width, oldImgData.Height,
                    Data.NewImage.Width, Data.NewImage.Height);

                if (resizeArgs.WidthFactor != 1 || resizeArgs.HeightFactor != 1)
                {
                    foreach (OptionItem item in OptionItems)
                        item.Shape.Resize(resizeArgs);

                    SetShapes();
                }
            }
        }

        private void UploadConfirmCancelButton_Click(object sender, EventArgs e)
        {
            Data.ConfirmImage = null;
            UploadConfirmImage.Src = null;

            MultiView.SetActiveView(UploadInputView);
        }

        private void Repeater_DataBinding(object sender, EventArgs e)
        {
            SetShapes();
        }

        private void ChangeImageButton_Click(object sender, EventArgs e)
        {
            UploadCancelButton.Visible = Data.ExistImage != null || Data.NewImage != null;

            MultiView.SetActiveView(UploadInputView);
        }

        private void AddRectangleOption_Click(object sender, EventArgs e)
        {
            var imgData = GetImage();
            var shape = new HotspotShapeRectangle(imgData.Width / 2 - 50, imgData.Height / 2 - 50, 100, 100);
            var item = new OptionItem(shape);

            OptionItems.Add(item);

            Repeater.DataBind();

            SelectShape(item.Sequence);
        }

        private void AddCircleOption_Click(object sender, EventArgs e)
        {
            var imgData = GetImage();
            var shape = new HotspotShapeCircle(imgData.Width / 2, imgData.Height / 2, 50);
            var item = new OptionItem(shape);

            OptionItems.Add(item);

            Repeater.DataBind();

            SelectShape(item.Sequence);
        }

        protected override void OnOptionItemDataBound(RepeaterItem repeaterItem, ItemCollection.DataItem optionItem) =>
            OnOptionItemDataBound(repeaterItem, (OptionItem)optionItem);

        private void OnOptionItemDataBound(RepeaterItem repeaterItem, OptionItem optionItem)
        {
            var textInput = (EditorTranslation)repeaterItem.FindControl("OptionText");
            var pointsInput = (NumericBox)repeaterItem.FindControl("Points");

            optionItem.Text.Set(textInput.Text);
            optionItem.Points = pointsInput.ValueAsDecimal ?? 0;
        }

        #endregion

        #region Methods (public)

        public override void LoadData()
        {
            Data = new ControlData();

            base.LoadData();

            UploadCancelButton.Visible = false;

            MultiView.SetActiveView(UploadInputView);
        }

        public override void LoadData(Question question)
        {
            Data = new ControlData
            {
                ExistImage = ImageData.Create(question.Hotspot.Image)
            };

            base.LoadData(question);

            MultiView.SetActiveView(ManageOptionsView);
        }

        public Hotspot GetHotspot(int bankAsset)
        {
            var result = new Hotspot();

            if (Data.NewImage != null)
            {
                var url = UploadNewImage(bankAsset);

                result.Image.Set(url, Data.NewImage.Width, Data.NewImage.Height);
            }
            else
            {
                result.Image.Set(Data.ExistImage.Path, Data.ExistImage.Width, Data.ExistImage.Height);
            }

            ApplyReorder();

            foreach (OptionItem item in OptionItems)
                result.AddOption(item.GetHotspotOption());

            return result;
        }

        public override Command[] GetCommands(Question question)
        {
            if (question == null)
                throw new ArgumentNullException(nameof(question));

            if (OptionItems == null)
                return null;

            var bank = question.Set.Bank;
            var commands = new List<Command>();

            if (Data.NewImage != null)
            {
                var url = UploadNewImage(bank.Asset);
                var image = new HotspotImage(url, Data.NewImage.Width, Data.NewImage.Height);

                commands.Add(new ChangeQuestionHotspotImage(bank.Identifier, question.Identifier, image));
            }

            ApplyReorder();

            var reorderData = new Dictionary<Guid, int>();
            var nextOptionIndex = question.Hotspot.Options.Count;

            for (var i = 0; i < OptionItems.Count; i++)
            {
                var item = (OptionItem)OptionItems[i];
                var option = item.GetHotspotOption();

                if (item.Number > 0)
                {
                    var existOption = question.Hotspot.GetOption(item.Identifier);
                    if (!option.IsEqual(existOption))
                        commands.Add(new ChangeQuestionHotspotOption(question.Set.Bank.Identifier, question.Identifier, existOption.Identifier, option.Shape, option.Content, option.Points));

                    if (existOption.Index != i)
                        reorderData.Add(existOption.Identifier, i);
                }
                else
                {
                    var newIndex = nextOptionIndex++;

                    commands.Add(new AddQuestionHotspotOption(question.Set.Bank.Identifier, question.Identifier, option.Identifier, option.Shape, option.Content, option.Points));

                    if (newIndex != i)
                        reorderData.Add(option.Identifier, i);
                }
            }

            if (reorderData.Count > 0)
                commands.Add(new ReorderQuestionHotspotOptions(bank.Identifier, question.Identifier, reorderData));

            foreach (var option in question.Hotspot.Options)
            {
                if (OptionItems.Any(o => o.Number == option.Number))
                    continue;

                if (!CanRemove(OptionItems.QuestionIdentifier, option.Number, out var removeError))
                    throw new ApplicationError(removeError);

                commands.Add(new DeleteQuestionHotspotOption(question.Set.Bank.Identifier, question.Identifier, option.Identifier));
            }

            return commands.ToArray();
        }

        public string GetError()
        {
            if (GetImage() == null)
                return "The question has no image.";

            if (OptionItems.Count == 0)
                return "The question has no options.";

            if (OptionItems.Cast<OptionItem>().All(x => x.Points == default))
                return "The question contains no correct option.";

            return null;
        }

        #endregion

        #region Methods (other)

        private ImageData GetImage() => Data.NewImage ?? Data.ExistImage;

        protected override void PopulateItems(Question question)
        {
            if (question == null)
                return;

            foreach (var option in question.Hotspot.Options)
                OptionItems.Add(new OptionItem(option));
        }

        private string UploadNewImage(int bankAsset)
        {
            if (Data.NewImage == null)
                return null;

            var sourceFilePath = ImageUpload.FilePath;

            var destFileName = $"{DateTime.UtcNow:yyyyMMdd-HHmmss}-{RandomStringGenerator.Create(4)}{Path.GetExtension(sourceFilePath)}";
            var destFilePath = $"/Assessments/{bankAsset}/Hotspots/{destFileName}";
            var destFileUrl = $"/files{destFilePath}";

            using (var stream = new FileStream(sourceFilePath, FileMode.Open))
                FileHelper.Provider.Save(Organization.Identifier, destFilePath, stream);

            return destFileUrl;
        }

        #endregion

        #region Methods (client-side)

        private void DrawImage()
        {
            var imgData = GetImage();
            var jsonData = JsonHelper.SerializeJsObject(imgData);

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(QuestionHotspotDetails),
                "init",
                $"hotspotDetails.drawImage({jsonData});",
                true
            );
        }

        private void SetShapes()
        {
            QuestionShapes.Value = string.Join(
                "|",
                OptionItems.Cast<OptionItem>().Select(x => x.Sequence + " " + x.Shape.ToString()));
        }

        private void GetShapes()
        {
            var shapes = QuestionShapes.Value.IsEmpty()
                ? new string[0]
                : QuestionShapes.Value.Split('|');

            if (shapes.Length != (OptionItems?.Count ?? 0))
                throw ApplicationError.Create("The number of shapes doesn't match the number of options");

            for (var i = 0; i < shapes.Length; i++)
            {
                var value = shapes[i];
                var item = (OptionItem)OptionItems[i];

                var idIndex = value.IndexOf(' ');
                if (idIndex == -1)
                    throw ApplicationError.Create("Invalid shape format: " + value);

                var shapeId = int.Parse(value.Substring(0, idIndex));
                if (shapeId != item.Sequence)
                    throw ApplicationError.Create("Invalid shape sequence: " + value);

                item.Shape.Read(value.Substring(idIndex + 1));
            }
        }

        private void SelectShape(int id)
        {
            ScriptManager.RegisterStartupScript(
                Page,
                typeof(QuestionHotspotDetails),
                "init",
                $"setTimeout(function () {{ hotspotDetails.selectShape({id}); }}, 0);",
                true
            );
        }

        #endregion
    }
}