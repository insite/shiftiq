using System.Net;
using System.Net.Http.Json;
using System.Security.Principal;
using System.Text;

using InSite.Application.Files.Read;
using InSite.Application.Responses.Write;
using InSite.Domain.Foundations;
using InSite.Domain.Organizations;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.File;
using Shift.Constant;
using Shift.Service.Content;

using PersonModel = InSite.Domain.Foundations.Person;
using UserModel = InSite.Domain.Foundations.User;

namespace Shift.Test.Content
{
    [Collection(TestFixtures.Default)]
    [Trait("Category", "Unit")]
    public class FilesTests
    {
        private const string BaseAddress = "https://localhost:5000";
        private const string StorageEndpoint = "content/files";

        private class Identity : ISecurityFramework
        {
            public ClaimList Claims
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            public GroupList Groups { get; set; }

            public Guid[] RoleIds
            {
                get
                {
                    if (Groups == null || Groups.Count == 0)
                        return new Guid[0];

                    return Groups.Select(x => x.Identifier).ToArray();
                }
            }

            public bool HasAccessToAllCompanies
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            public Impersonator Impersonator
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            public bool IsAdministrator
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            public bool IsDeveloper
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            public bool IsOperator
            {
                get => false;
            }

            public bool IsImpersonating
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            public string Language
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            public OrganizationState Organization { get; set; }

            public Guid? OrganizationId => Organization?.OrganizationIdentifier;

            public OrganizationList Organizations
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            public PersonModel Person
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            public PersonList Persons
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            public UserModel User { get; set; }

            public Guid? UserId => User?.UserIdentifier;

            public string Name => throw new NotImplementedException();

            public string AuthenticationType => throw new NotImplementedException();

            public bool IsAuthenticated => throw new NotImplementedException();

            IIdentity System.Security.Principal.IPrincipal.Identity => throw new NotImplementedException();

            public string ChangeLanguage(string language) => throw new NotImplementedException();
            public bool IsActionAuthorized(string actionName) => throw new NotImplementedException();
            public bool IsGranted(Guid? action) => throw new NotImplementedException();
            public bool IsGranted(Guid? action, PermissionOperation operation) => throw new NotImplementedException();
            public bool IsGranted(string action) => throw new NotImplementedException();
            public bool IsGranted(string action, PermissionOperation operation) => throw new NotImplementedException();
            public bool IsInGroup(string group) => throw new NotImplementedException();
            public bool IsInGroup(string[] groups) => throw new NotImplementedException();
            public bool IsInGroup(Guid group) => throw new NotImplementedException();

            public bool IsInRole(string role)
            {
                throw new NotImplementedException();
            }

            public Identity(Guid organizationId, Guid userId, Guid[] groups)
            {
                User = new UserModel { Identifier = userId };
                Groups = new GroupList(groups.Select(x => new Group { Identifier = x }).ToArray());
                Organization = new OrganizationState { OrganizationIdentifier = organizationId };
            }
        }

        private static readonly Guid GlobalId = TestFixture.Organization1!.Id;
        private static readonly string GlobalCode = TestFixture.Organization1.Code;
        private static readonly Guid InSiteId = TestFixture.Organization2!.Id;
        private static readonly string InSiteCode = TestFixture.Organization2.Code;

        private static readonly Guid UserId1 = Guid.NewGuid();
        private static readonly Guid UserId2 = Guid.NewGuid();

        private static readonly Guid[] GroupsOfUser2 = new[] { GroupIdentifiers.PlatformAdministrator };

        private readonly TestFixture _fixture;
        private readonly string _domain;

        public FilesTests(TestFixture fixture)
        {
            _fixture = fixture;
            _domain = _fixture.Settings.Security.Domain;
        }

        [Fact]
        public async Task Files_UploadPublic_Success()
        {
            var data = Encoding.UTF8.GetBytes("Hello world");

            var props = new FileProperties
            {
                DocumentName = "Document N1",
                Description = "Document Description",
                Category = "Document Category",
                Subcategory = "Document Subcategory",
                Status = "Requested",
                Expiry = DateTimeOffset.UtcNow.AddDays(1),
                Received = DateTimeOffset.UtcNow.AddDays(-1),
                Alternated = DateTimeOffset.UtcNow,
                ReviewedTime = DateTimeOffset.UtcNow.AddDays(-0.6),
                ReviewedUserIdentifier = Guid.NewGuid(),
                ApprovedTime = DateTimeOffset.UtcNow.AddDays(-0.5),
                ApprovedUserIdentifier = Guid.NewGuid()
            };

            FileStorageModel model;

            using (var file = new MemoryStream(data))
            {
                model = await _fixture.StorageService.CreateAsync(
                    file,
                    "Document File.pptx",
                    GlobalId,
                    UserId1,
                    UserId1,
                    FileObjectType.User,
                    props,
                    null
                );
            }

            var status = await _fixture.StorageService.GetGrantStatusAsync(new Identity(GlobalId, Guid.Empty, []), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.Granted, status);

            var (status2, model2) = await _fixture.StorageService.GetFileAndAuthorizeAsync(new Identity(GlobalId, Guid.Empty, []), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.Granted, status2);

            var props2 = model2.Properties;

            var fileUrlExpected = $"{StorageEndpoint}/{model.FileIdentifier}/{model.FileName}".ToLower();
            var fileUrlActual = _fixture.StorageService.GetFileUrl(model2.FileIdentifier, model2.FileName);
            Assert.Equal(fileUrlExpected, fileUrlActual);

            Assert.Equal(model.FileName, model2.FileName);
            Assert.Equal(model.UserIdentifier, model2.UserIdentifier);
            Assert.Equal(model.OrganizationIdentifier, model2.OrganizationIdentifier);
            Assert.Equal(model.ObjectIdentifier, model2.ObjectIdentifier);
            Assert.Equal(model.ObjectType, model2.ObjectType);
            Assert.Equal(model.FileSize, model2.FileSize);
            Assert.Equal(FileLocation.Local, model2.FileLocation);
            Assert.Equal(model.FileLocation, model2.FileLocation);
            Assert.Equal(model.ContentType, model2.ContentType);

            Assert.Equal(props.DocumentName, props2.DocumentName);
            Assert.Equal(props.Description, props2.Description);
            Assert.Equal(props.Category, props2.Category);
            Assert.Equal(props.Subcategory, props2.Subcategory);
            Assert.Equal(props.Status, props2.Status);
            Assert.Equal(props.Expiry, props2.Expiry);
            Assert.Equal(props.Received, props2.Received);
            Assert.Equal(props.Alternated, props2.Alternated);
            Assert.Equal(props.ReviewedTime, props2.ReviewedTime);
            Assert.Equal(props.ReviewedUserIdentifier, props2.ReviewedUserIdentifier);
            Assert.Equal(props.ApprovedTime, props2.ApprovedTime);
            Assert.Equal(props.ApprovedUserIdentifier, props2.ApprovedUserIdentifier);

            _fixture.StorageService.ClearCache();

            var model3 = await _fixture.StorageService.GetFileAsync(model.FileIdentifier);
            var props3 = model3.Properties;

            Assert.Equal(model.FileName, model3.FileName);
            Assert.Equal(model.UserIdentifier, model3.UserIdentifier);
            Assert.Equal(model.OrganizationIdentifier, model3.OrganizationIdentifier);
            Assert.Equal(model.ObjectIdentifier, model3.ObjectIdentifier);
            Assert.Equal(model.ObjectType, model3.ObjectType);
            Assert.Equal(model.FileSize, model3.FileSize);
            Assert.Equal(model.ContentType, model3.ContentType);

            Assert.Equal(props.DocumentName, props3.DocumentName);
            Assert.Equal(props.Description, props3.Description);
            Assert.Equal(props.Category, props3.Category);
            Assert.Equal(props.Subcategory, props3.Subcategory);
            Assert.Equal(props.Status, props3.Status);
            Assert.Equal(props.Expiry, props3.Expiry);
            Assert.Equal(props.Received, props3.Received);
            Assert.Equal(props.Alternated, props3.Alternated);
            Assert.Equal(props.ReviewedTime, props3.ReviewedTime);
            Assert.Equal(props.ReviewedUserIdentifier, props3.ReviewedUserIdentifier);
            Assert.Equal(props.ApprovedTime, props3.ApprovedTime);
            Assert.Equal(props.ApprovedUserIdentifier, props3.ApprovedUserIdentifier);

            Assert.True(_fixture.FileManagerService.IsFileExist(model.OrganizationIdentifier, model.FileIdentifier, model.FileName, model.FilePath));

            await _fixture.StorageService.DeleteAsync(model.FileIdentifier);

            Assert.False(_fixture.FileManagerService.IsFileExist(model.OrganizationIdentifier, model.FileIdentifier, model.FileName, model.FilePath));
        }

        [Fact]
        public async Task Files_UploadPrivateUser_Success()
        {
            var data = Encoding.UTF8.GetBytes("Hello world (Private User)");

            var props = new FileProperties
            {
                DocumentName = "Document N2"
            };

            FileStorageModel model;

            using (var file = new MemoryStream(data))
            {
                model = await _fixture.StorageService.CreateAsync(
                    file,
                    "Document File 2.docx",
                    GlobalId,
                    UserId2,
                    UserId2,
                    FileObjectType.User,
                    props,
                    new FileClaim[]
                    {
                        new FileClaim { ObjectIdentifier = UserId1, ObjectType = FileClaimObjectType.Person }
                    }
                );
            }

            var status1 = await _fixture.StorageService.GetGrantStatusAsync(new Identity(GlobalId, UserId2, GroupsOfUser2), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.Denied, status1);

            var (status12, _) = await _fixture.StorageService.GetFileAndAuthorizeAsync(new Identity(GlobalId, UserId2, GroupsOfUser2), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.Denied, status12);

            var status2 = await _fixture.StorageService.GetGrantStatusAsync(new Identity(GlobalId, UserId1, []), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.Granted, status2);

            var (status22, _) = await _fixture.StorageService.GetFileAndAuthorizeAsync(new Identity(GlobalId, UserId1, []), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.Granted, status22);

            var status3 = await _fixture.StorageService.GetGrantStatusAsync(new Identity(InSiteId, UserId1, []), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.NoFile, status3);

            var (status32, _) = await _fixture.StorageService.GetFileAndAuthorizeAsync(new Identity(InSiteId, UserId1, []), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.NoFile, status32);

            await _fixture.StorageService.DeleteAsync(model.FileIdentifier);
        }

        [Fact]
        public async Task Files_UploadPrivateGroup_Success()
        {
            var data = Encoding.UTF8.GetBytes("Hello world (Private Group)");

            var props = new FileProperties
            {
                DocumentName = "Document N3"
            };

            FileStorageModel model;

            using (var file = new MemoryStream(data))
            {
                model = await _fixture.StorageService.CreateAsync(
                    file,
                    "Document File 3.txt",
                    GlobalId,
                    UserId2,
                    UserId2,
                    FileObjectType.User,
                    props,
                    new FileClaim[]
                    {
                        new FileClaim { ObjectIdentifier = Shift.Constant.GroupIdentifiers.PlatformAdministrator, ObjectType = FileClaimObjectType.Group }
                    }
                );
            }

            var status1 = await _fixture.StorageService.GetGrantStatusAsync(new Identity(GlobalId, UserId2, GroupsOfUser2), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.Granted, status1);

            var (status12, _) = await _fixture.StorageService.GetFileAndAuthorizeAsync(new Identity(GlobalId, UserId2, GroupsOfUser2), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.Granted, status12);

            var status2 = await _fixture.StorageService.GetGrantStatusAsync(new Identity(GlobalId, UserId1, []), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.Denied, status2);

            var (status22, _) = await _fixture.StorageService.GetFileAndAuthorizeAsync(new Identity(GlobalId, UserId1, []), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.Denied, status22);

            var status3 = await _fixture.StorageService.GetGrantStatusAsync(new Identity(InSiteId, UserId2, GroupsOfUser2), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.NoFile, status3);

            var (status32, _) = await _fixture.StorageService.GetFileAndAuthorizeAsync(new Identity(InSiteId, UserId2, GroupsOfUser2), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.NoFile, status32);

            await _fixture.StorageService.DeleteAsync(model.FileIdentifier);
        }

        [Fact]
        public async Task Files_Delete_Success()
        {
            var data = Encoding.UTF8.GetBytes("Hello world (Delete)");

            var props = new FileProperties
            {
                DocumentName = "Document N4"
            };

            FileStorageModel model;

            using (var file = new MemoryStream(data))
            {
                model = await _fixture.StorageService.CreateAsync(
                    file,
                    "Document File 4.txt",
                    GlobalId,
                    UserId1,
                    UserId1,
                    FileObjectType.User,
                    props,
                    null
                );
            }

            Assert.NotNull(model);

            await _fixture.StorageService.DeleteAsync(model.FileIdentifier);

            var status = await _fixture.StorageService.GetGrantStatusAsync(new Identity(GlobalId, Guid.Empty, []), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.NoFile, status);
        }

        [Fact]
        public async Task Files_ChangeProps_Success()
        {
            var data = Encoding.UTF8.GetBytes("Hello world (Change Props)");

            var props = new FileProperties
            {
                DocumentName = "Document N5",
                Description = "Document Description",
                Category = "Document Category",
                Subcategory = "Document Subcategory",
                Status = "Requested",
                Expiry = DateTimeOffset.UtcNow.AddDays(1),
                Received = DateTimeOffset.UtcNow.AddDays(-1),
                Alternated = DateTimeOffset.UtcNow,
            };

            FileStorageModel model;

            using (var file = new MemoryStream(data))
            {
                model = await _fixture.StorageService.CreateAsync(
                    file,
                    "Document File 5.txt",
                    GlobalId,
                    UserId1,
                    UserId1,
                    FileObjectType.User,
                    props,
                    null
                );
            }

            var newProps = new FileProperties
            {
                DocumentName = "Document N5 (Changed)",
                Description = "Document Description (Changed)",
                Category = "Document Category (Changed)",
                Subcategory = "Document Subcategory (Changed)",
                Status = "Uploaded",
                Expiry = DateTimeOffset.UtcNow.AddDays(2),
                Received = DateTimeOffset.UtcNow.AddDays(-2),
                Alternated = DateTimeOffset.UtcNow.AddDays(1),
                ReviewedTime = DateTimeOffset.UtcNow.AddDays(-0.6),
                ReviewedUserIdentifier = Guid.NewGuid(),
                ApprovedTime = DateTimeOffset.UtcNow.AddDays(-0.5),
                ApprovedUserIdentifier = Guid.NewGuid()
            };

            await _fixture.StorageService.ChangePropertiesAsync(model.FileIdentifier, UserId2, newProps, true);

            var model2 = await _fixture.StorageService.GetFileAsync(model.FileIdentifier);
            var props2 = model2.Properties;

            Assert.Equal(newProps.DocumentName, props2.DocumentName);
            Assert.Equal(newProps.Description, props2.Description);
            Assert.Equal(newProps.Category, props2.Category);
            Assert.Equal(newProps.Subcategory, props2.Subcategory);
            Assert.Equal(newProps.Status, props2.Status);
            Assert.Equal(newProps.Expiry, props2.Expiry);
            Assert.Equal(newProps.Received, props2.Received);
            Assert.Equal(newProps.Alternated, props2.Alternated);
            Assert.Equal(newProps.ReviewedTime, props2.ReviewedTime);
            Assert.Equal(newProps.ReviewedUserIdentifier, props2.ReviewedUserIdentifier);
            Assert.Equal(newProps.ApprovedTime, props2.ApprovedTime);
            Assert.Equal(newProps.ApprovedUserIdentifier, props2.ApprovedUserIdentifier);

            _fixture.StorageService.ClearCache();

            var model3 = await _fixture.StorageService.GetFileAsync(model.FileIdentifier);
            var props3 = model3.Properties;

            Assert.Equal(newProps.DocumentName, props3.DocumentName);
            Assert.Equal(newProps.Description, props3.Description);
            Assert.Equal(newProps.Category, props3.Category);
            Assert.Equal(newProps.Subcategory, props3.Subcategory);
            Assert.Equal(newProps.Status, props3.Status);
            Assert.Equal(newProps.Expiry, props3.Expiry);
            Assert.Equal(newProps.Received, props3.Received);
            Assert.Equal(newProps.Alternated, props3.Alternated);
            Assert.Equal(newProps.ReviewedTime, props3.ReviewedTime);
            Assert.Equal(newProps.ReviewedUserIdentifier, props3.ReviewedUserIdentifier);
            Assert.Equal(newProps.ApprovedTime, props3.ApprovedTime);
            Assert.Equal(newProps.ApprovedUserIdentifier, props3.ApprovedUserIdentifier);

            await _fixture.StorageService.DeleteAsync(model.FileIdentifier);
        }

        [Fact]
        public async Task Files_ChangeObject_Success()
        {
            var data = Encoding.UTF8.GetBytes("Hello world (Change Object)");
            var initialObjectId = Guid.NewGuid();

            FileStorageModel model;

            using (var file = new MemoryStream(data))
            {
                model = await _fixture.StorageService.CreateAsync(
                    file,
                    "Document File 5.1.txt",
                    GlobalId,
                    UserId1,
                    initialObjectId,
                    FileObjectType.Temporary,
                    null,
                    null
                );
            }

            await _fixture.StorageService.ChangeObjectAsync(model.FileIdentifier, UserId1, FileObjectType.User);

            var model2 = await _fixture.StorageService.GetFileAsync(model.FileIdentifier);

            Assert.Equal(UserId1, model2.ObjectIdentifier);
            Assert.Equal(FileObjectType.User, model2.ObjectType);

            _fixture.StorageService.ClearCache();

            var model3 = await _fixture.StorageService.GetFileAsync(model.FileIdentifier);

            Assert.Equal(UserId1, model3.ObjectIdentifier);
            Assert.Equal(FileObjectType.User, model3.ObjectType);

            await _fixture.StorageService.DeleteAsync(model.FileIdentifier);
        }


        [Fact]
        public async Task Files_ChangeClaimsWithCache_Success()
        {
            var data = Encoding.UTF8.GetBytes("Hello world (Change Claims With Cache)");

            var props = new FileProperties
            {
                DocumentName = "Document N6"
            };

            FileStorageModel model;

            using (var file = new MemoryStream(data))
            {
                model = await _fixture.StorageService.CreateAsync(
                    file,
                    "Document File 6.txt",
                    GlobalId,
                    UserId1,
                    UserId1,
                    FileObjectType.User,
                    props,
                    new FileClaim[]
                    {
                        new FileClaim { ObjectIdentifier = Shift.Constant.GroupIdentifiers.PlatformAdministrator, ObjectType = FileClaimObjectType.Group }
                    }
                );
            }

            var status1 = await _fixture.StorageService.GetGrantStatusAsync(new Identity(GlobalId, UserId2, GroupsOfUser2), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.Granted, status1);

            // By User

            await _fixture.StorageService.ChangeClaimsAsync(model.FileIdentifier, new FileClaim[]
            {
                new FileClaim { ObjectIdentifier = UserId1, ObjectType = FileClaimObjectType.Person }
            });

            var status2 = await _fixture.StorageService.GetGrantStatusAsync(new Identity(GlobalId, UserId2, GroupsOfUser2), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.Denied, status2);

            var status3 = await _fixture.StorageService.GetGrantStatusAsync(new Identity(GlobalId, UserId1, []), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.Granted, status3);

            // By User and Group

            await _fixture.StorageService.ChangeClaimsAsync(model.FileIdentifier, new FileClaim[]
            {
                new FileClaim { ObjectIdentifier = UserId1, ObjectType = FileClaimObjectType.Person },
                new FileClaim { ObjectIdentifier = Shift.Constant.GroupIdentifiers.PlatformAdministrator, ObjectType = FileClaimObjectType.Group }
            });

            var status4 = await _fixture.StorageService.GetGrantStatusAsync(new Identity(GlobalId, UserId2, GroupsOfUser2), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.Granted, status4);

            var status5 = await _fixture.StorageService.GetGrantStatusAsync(new Identity(GlobalId, UserId1, []), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.Granted, status5);

            // By Group

            await _fixture.StorageService.ChangeClaimsAsync(model.FileIdentifier, new FileClaim[]
            {
                new FileClaim { ObjectIdentifier = Shift.Constant.GroupIdentifiers.PlatformAdministrator, ObjectType = FileClaimObjectType.Group }
            });

            var status6 = await _fixture.StorageService.GetGrantStatusAsync(new Identity(GlobalId, UserId2, GroupsOfUser2), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.Granted, status6);

            var status7 = await _fixture.StorageService.GetGrantStatusAsync(new Identity(GlobalId, UserId1, []), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.Denied, status7);

            await _fixture.StorageService.DeleteAsync(model.FileIdentifier);
        }

        [Fact]
        public async Task Files_ChangeClaimsNoCache_Success()
        {
            var data = Encoding.UTF8.GetBytes("Hello world (Change Claims No Cache)");

            var props = new FileProperties
            {
                DocumentName = "Document N7"
            };

            FileStorageModel model;

            using (var file = new MemoryStream(data))
            {
                model = await _fixture.StorageService.CreateAsync(
                    file,
                    "Document File 7.txt",
                    GlobalId,
                    UserId1,
                    UserId1,
                    FileObjectType.User,
                    props,
                    new FileClaim[]
                    {
                        new FileClaim { ObjectIdentifier = Shift.Constant.GroupIdentifiers.PlatformAdministrator, ObjectType = FileClaimObjectType.Group }
                    }
                );
            }

            _fixture.StorageService.ClearCache();

            var status1 = await _fixture.StorageService.GetGrantStatusAsync(new Identity(GlobalId, UserId2, GroupsOfUser2), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.Granted, status1);

            // By User

            await _fixture.StorageService.ChangeClaimsAsync(model.FileIdentifier, new FileClaim[]
            {
                new FileClaim { ObjectIdentifier = UserId1, ObjectType = FileClaimObjectType.Person }
            });

            _fixture.StorageService.ClearCache();

            var status2 = await _fixture.StorageService.GetGrantStatusAsync(new Identity(GlobalId, UserId2, GroupsOfUser2), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.Denied, status2);

            var status3 = await _fixture.StorageService.GetGrantStatusAsync(new Identity(GlobalId, UserId1, []), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.Granted, status3);

            // By User and Group

            await _fixture.StorageService.ChangeClaimsAsync(model.FileIdentifier, new FileClaim[]
            {
                new FileClaim { ObjectIdentifier = UserId1, ObjectType = FileClaimObjectType.Person },
                new FileClaim { ObjectIdentifier = Shift.Constant.GroupIdentifiers.PlatformAdministrator, ObjectType = FileClaimObjectType.Group }
            });

            _fixture.StorageService.ClearCache();

            var status4 = await _fixture.StorageService.GetGrantStatusAsync(new Identity(GlobalId, UserId2, GroupsOfUser2), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.Granted, status4);

            var status5 = await _fixture.StorageService.GetGrantStatusAsync(new Identity(GlobalId, UserId1, []), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.Granted, status5);

            // By Group

            await _fixture.StorageService.ChangeClaimsAsync(model.FileIdentifier, new FileClaim[]
            {
                new FileClaim { ObjectIdentifier = Shift.Constant.GroupIdentifiers.PlatformAdministrator, ObjectType = FileClaimObjectType.Group }
            });

            _fixture.StorageService.ClearCache();

            var status6 = await _fixture.StorageService.GetGrantStatusAsync(new Identity(GlobalId, UserId2, GroupsOfUser2), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.Granted, status6);

            var status7 = await _fixture.StorageService.GetGrantStatusAsync(new Identity(GlobalId, UserId1, []), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.Denied, status7);

            await _fixture.StorageService.DeleteAsync(model.FileIdentifier);
        }

        [Fact]
        public async Task Files_FileStreamWithCache_Success()
        {
            var text = "Hello world (FileStream with Cache)";
            var data = Encoding.UTF8.GetBytes(text);

            var props = new FileProperties
            {
                DocumentName = "Document N8",
                Description = "Document Description",
                Category = "Document Category",
                Subcategory = "Document Subcategory",
                Status = "Requested",
                Expiry = DateTimeOffset.UtcNow.AddDays(1),
                Received = DateTimeOffset.UtcNow.AddDays(-1),
                Alternated = DateTimeOffset.UtcNow,
                ReviewedTime = DateTimeOffset.UtcNow.AddDays(-0.6),
                ReviewedUserIdentifier = Guid.NewGuid(),
                ApprovedTime = DateTimeOffset.UtcNow.AddDays(-0.5),
                ApprovedUserIdentifier = Guid.NewGuid()
            };

            FileStorageModel model;

            using (var file = new MemoryStream(data))
            {
                model = await _fixture.StorageService.CreateAsync(
                    file,
                    "Document File 8.txt",
                    GlobalId,
                    UserId1,
                    UserId1,
                    FileObjectType.User,
                    props,
                    null
                );
            }

            var (model2, stream2) = await _fixture.StorageService.GetFileStreamAsync(model.FileIdentifier);
            var props2 = model2.Properties;

            string actualText;
            try
            {
                using (var reader = new StreamReader(stream2))
                    actualText = reader.ReadToEnd();
            }
            finally
            {
                stream2.Close();
            }

            Assert.Equal(text, actualText);

            Assert.Equal(model.FileName, model2.FileName);
            Assert.Equal(model.UserIdentifier, model2.UserIdentifier);
            Assert.Equal(model.OrganizationIdentifier, model2.OrganizationIdentifier);
            Assert.Equal(model.ObjectIdentifier, model2.ObjectIdentifier);
            Assert.Equal(model.ObjectType, model2.ObjectType);
            Assert.Equal(model.FileSize, model2.FileSize);
            Assert.Equal(model.ContentType, model2.ContentType);

            Assert.Equal(props.DocumentName, props2.DocumentName);
            Assert.Equal(props.Description, props2.Description);
            Assert.Equal(props.Category, props2.Category);
            Assert.Equal(props.Subcategory, props2.Subcategory);
            Assert.Equal(props.Status, props2.Status);
            Assert.Equal(props.Expiry, props2.Expiry);
            Assert.Equal(props.Received, props2.Received);
            Assert.Equal(props.Alternated, props2.Alternated);
            Assert.Equal(props.ReviewedTime, props2.ReviewedTime);
            Assert.Equal(props.ReviewedUserIdentifier, props2.ReviewedUserIdentifier);
            Assert.Equal(props.ApprovedTime, props2.ApprovedTime);
            Assert.Equal(props.ApprovedUserIdentifier, props2.ApprovedUserIdentifier);

            await _fixture.StorageService.DeleteAsync(model.FileIdentifier);
        }

        [Fact]
        public async Task Files_FileStreamWithCache2_Success()
        {
            var text = "Hello world (FileStream with Cache)";
            var data = Encoding.UTF8.GetBytes(text);

            var props = new FileProperties
            {
                DocumentName = "Document N8",
                Description = "Document Description",
                Category = "Document Category",
                Subcategory = "Document Subcategory",
                Status = "Requested",
                Expiry = DateTimeOffset.UtcNow.AddDays(1),
                Received = DateTimeOffset.UtcNow.AddDays(-1),
                Alternated = DateTimeOffset.UtcNow,
                ReviewedTime = DateTimeOffset.UtcNow.AddDays(-0.6),
                ReviewedUserIdentifier = Guid.NewGuid(),
                ApprovedTime = DateTimeOffset.UtcNow.AddDays(-0.5),
                ApprovedUserIdentifier = Guid.NewGuid()
            };

            FileStorageModel model;

            using (var file = new MemoryStream(data))
            {
                model = await _fixture.StorageService.CreateAsync(
                    file,
                    "Document File 8.txt",
                    GlobalId,
                    UserId1,
                    UserId1,
                    FileObjectType.User,
                    props,
                    null
                );
            }

            var (status2, model2, stream2) = await _fixture.StorageService.GetFileStreamAndAuthorizeAsync(new Identity(GlobalId, Guid.Empty, []), model.FileIdentifier);
            Assert.Equal(FileGrantStatus.Granted, status2);

            var props2 = model2.Properties;

            string actualText;
            try
            {
                using (var reader = new StreamReader(stream2))
                    actualText = reader.ReadToEnd();
            }
            finally
            {
                stream2.Close();
            }

            Assert.Equal(text, actualText);

            Assert.Equal(model.FileName, model2.FileName);
            Assert.Equal(model.UserIdentifier, model2.UserIdentifier);
            Assert.Equal(model.OrganizationIdentifier, model2.OrganizationIdentifier);
            Assert.Equal(model.ObjectIdentifier, model2.ObjectIdentifier);
            Assert.Equal(model.ObjectType, model2.ObjectType);
            Assert.Equal(model.FileSize, model2.FileSize);
            Assert.Equal(model.ContentType, model2.ContentType);

            Assert.Equal(props.DocumentName, props2.DocumentName);
            Assert.Equal(props.Description, props2.Description);
            Assert.Equal(props.Category, props2.Category);
            Assert.Equal(props.Subcategory, props2.Subcategory);
            Assert.Equal(props.Status, props2.Status);
            Assert.Equal(props.Expiry, props2.Expiry);
            Assert.Equal(props.Received, props2.Received);
            Assert.Equal(props.Alternated, props2.Alternated);
            Assert.Equal(props.ReviewedTime, props2.ReviewedTime);
            Assert.Equal(props.ReviewedUserIdentifier, props2.ReviewedUserIdentifier);
            Assert.Equal(props.ApprovedTime, props2.ApprovedTime);
            Assert.Equal(props.ApprovedUserIdentifier, props2.ApprovedUserIdentifier);

            await _fixture.StorageService.DeleteAsync(model.FileIdentifier);
        }

        [Fact]
        public async Task Files_FileStreamNoCache_Success()
        {
            var text = "Hello world (FileStream No Cache)";
            var data = Encoding.UTF8.GetBytes(text);

            var props = new FileProperties
            {
                DocumentName = "Document N9",
                Description = "Document Description",
                Category = "Document Category",
                Subcategory = "Document Subcategory",
                Status = "Requested",
                Expiry = DateTimeOffset.UtcNow.AddDays(1),
                Received = DateTimeOffset.UtcNow.AddDays(-1),
                Alternated = DateTimeOffset.UtcNow,
                ReviewedTime = DateTimeOffset.UtcNow.AddDays(-0.6),
                ReviewedUserIdentifier = Guid.NewGuid(),
                ApprovedTime = DateTimeOffset.UtcNow.AddDays(-0.5),
                ApprovedUserIdentifier = Guid.NewGuid()
            };

            FileStorageModel model;

            using (var file = new MemoryStream(data))
            {
                model = await _fixture.StorageService.CreateAsync(
                    file,
                    "Document File 9.txt",
                    GlobalId,
                    UserId1,
                    UserId1,
                    FileObjectType.User,
                    props,
                    null
                );
            }

            _fixture.StorageService.ClearCache();

            var (model2, stream) = await _fixture.StorageService.GetFileStreamAsync(model.FileIdentifier);
            var props2 = model2.Properties;

            string actualText;
            try
            {
                using (var reader = new StreamReader(stream))
                    actualText = reader.ReadToEnd();
            }
            finally
            {
                stream.Close();
            }

            Assert.Equal(text, actualText);

            Assert.Equal(model.FileName, model2.FileName);
            Assert.Equal(model.UserIdentifier, model2.UserIdentifier);
            Assert.Equal(model.OrganizationIdentifier, model2.OrganizationIdentifier);
            Assert.Equal(model.ObjectIdentifier, model2.ObjectIdentifier);
            Assert.Equal(model.ObjectType, model2.ObjectType);
            Assert.Equal(model.FileSize, model2.FileSize);
            Assert.Equal(model.ContentType, model2.ContentType);

            Assert.Equal(props.DocumentName, props2.DocumentName);
            Assert.Equal(props.Description, props2.Description);
            Assert.Equal(props.Category, props2.Category);
            Assert.Equal(props.Subcategory, props2.Subcategory);
            Assert.Equal(props.Status, props2.Status);
            Assert.Equal(props.Expiry, props2.Expiry);
            Assert.Equal(props.Received, props2.Received);
            Assert.Equal(props.Alternated, props2.Alternated);
            Assert.Equal(props.ReviewedTime, props2.ReviewedTime);
            Assert.Equal(props.ReviewedUserIdentifier, props2.ReviewedUserIdentifier);
            Assert.Equal(props.ApprovedTime, props2.ApprovedTime);
            Assert.Equal(props.ApprovedUserIdentifier, props2.ApprovedUserIdentifier);

            await _fixture.StorageService.DeleteAsync(model.FileIdentifier);
        }

        [Fact]
        public async Task Files_GrantedFilesByObject_Success()
        {
            var text = "Hello world (FileStream with Cache)";
            var data = Encoding.UTF8.GetBytes(text);

            var props = new FileProperties
            {
                DocumentName = "Document N8",
                Description = "Document Description",
                Category = "Document Category",
                Subcategory = "Document Subcategory",
                Status = "Requested",
                Expiry = DateTimeOffset.UtcNow.AddDays(1),
                Received = DateTimeOffset.UtcNow.AddDays(-1),
                Alternated = DateTimeOffset.UtcNow,
                ReviewedTime = DateTimeOffset.UtcNow.AddDays(-0.6),
                ReviewedUserIdentifier = Guid.NewGuid(),
                ApprovedTime = DateTimeOffset.UtcNow.AddDays(-0.5),
                ApprovedUserIdentifier = Guid.NewGuid()
            };

            FileStorageModel model1, model2, model3;

            using (var file = new MemoryStream(data))
            {
                model1 = await _fixture.StorageService.CreateAsync(
                    file,
                    "Document File 8.txt",
                    GlobalId,
                    UserId1,
                    UserId1,
                    FileObjectType.User,
                    props,
                    null
                );
            }

            using (var file = new MemoryStream(data))
            {
                model2 = await _fixture.StorageService.CreateAsync(
                    file,
                    "Document File 8.2.txt",
                    GlobalId,
                    UserId1,
                    UserId1,
                    FileObjectType.User,
                    props,
                    new FileClaim[]
                    {
                        new FileClaim { ObjectIdentifier = UserId1, ObjectType = FileClaimObjectType.Person }
                    }
                );
            }

            using (var file = new MemoryStream(data))
            {
                model3 = await _fixture.StorageService.CreateAsync(
                    file,
                    "Document File 8.2.txt",
                    GlobalId,
                    UserId1,
                    UserId1,
                    FileObjectType.User,
                    props,
                    new FileClaim[]
                    {
                        new FileClaim { ObjectIdentifier = UserId2, ObjectType = FileClaimObjectType.Person }
                    }
                );
            }

            var list1 = await _fixture.StorageService.GetGrantedFilesAsync(new Identity(GlobalId, Guid.Empty, []), UserId1);
            Assert.Single(list1);

            var list2 = await _fixture.StorageService.GetGrantedFilesAsync(new Identity(GlobalId, UserId1, []), UserId1);
            Assert.Equal(2, list2?.Count ?? 2);

            var model4 = list2!.First(x => x.FileIdentifier == model1.FileIdentifier);
            var props4 = model4.Properties;

            Assert.Equal(model1.FileName, model4.FileName);
            Assert.Equal(model1.UserIdentifier, model4.UserIdentifier);
            Assert.Equal(model1.OrganizationIdentifier, model4.OrganizationIdentifier);
            Assert.Equal(model1.ObjectIdentifier, model4.ObjectIdentifier);
            Assert.Equal(model1.ObjectType, model4.ObjectType);
            Assert.Equal(model1.FileSize, model4.FileSize);
            Assert.Equal(model1.ContentType, model4.ContentType);

            Assert.Equal(props.DocumentName, props4.DocumentName);
            Assert.Equal(props.Description, props4.Description);
            Assert.Equal(props.Category, props4.Category);
            Assert.Equal(props.Subcategory, props4.Subcategory);
            Assert.Equal(props.Status, props4.Status);
            Assert.Equal(props.Expiry, props4.Expiry);
            Assert.Equal(props.Received, props4.Received);
            Assert.Equal(props.Alternated, props4.Alternated);
            Assert.Equal(props.ReviewedTime, props4.ReviewedTime);
            Assert.Equal(props.ReviewedUserIdentifier, props4.ReviewedUserIdentifier);
            Assert.Equal(props.ApprovedTime, props4.ApprovedTime);
            Assert.Equal(props.ApprovedUserIdentifier, props4.ApprovedUserIdentifier);

            var model5 = list2!.First(x => x.FileIdentifier == model2.FileIdentifier);
            Assert.Equal(model2.FileName, model5.FileName);

            await _fixture.StorageService.DeleteAsync(model1.FileIdentifier);
            await _fixture.StorageService.DeleteAsync(model2.FileIdentifier);
            await _fixture.StorageService.DeleteAsync(model3.FileIdentifier);
        }

        [Fact]
        public async Task Files_StorageControllerPublic_Success()
        {
            var types = new (string extension, string mediaType)[]
            {
                (".txt", "text/plain"),
                (".png", "image/png"),
                (".jpg", "image/jpeg"),
                (".pdf", "application/pdf"),
            };

            foreach (var (extension, mediaType) in types)
            {
                var text = "Hello world (Storage Controller)";
                var data = Encoding.UTF8.GetBytes(text);

                var props = new FileProperties
                {
                    DocumentName = "Document N10",
                    Description = "Document Description",
                    Category = "Document Category",
                    Subcategory = "Document Subcategory",
                    Status = "Requested",
                    Expiry = DateTimeOffset.UtcNow.AddDays(1),
                    Received = DateTimeOffset.UtcNow.AddDays(-1),
                    Alternated = DateTimeOffset.UtcNow,
                };

                FileStorageModel model;

                using (var file = new MemoryStream(data))
                {
                    model = await _fixture.StorageService.CreateAsync(
                        file,
                        "Document File 10" + extension,
                        Guid.Empty,
                        UserId1,
                        UserId1,
                        FileObjectType.User,
                        props,
                        null
                    );
                }

                var relativeUrl = _fixture.StorageService.GetFileUrl(model.FileIdentifier, model.FileName);

                var absoluteUrl = $"{BaseAddress}/{relativeUrl}";

                var response = await StaticHttpClient.Client.GetAsync(absoluteUrl);

                response.EnsureSuccessStatusCode();

                Assert.Equal(mediaType, response.Content.Headers.ContentType!.MediaType, ignoreCase: true);

                var actualText = await response.Content.ReadAsStringAsync();

                Assert.Equal(text, actualText);

                await _fixture.StorageService.DeleteAsync(model.FileIdentifier);
            }
        }


        [Fact]
        public async Task Files_StorageControllerPrivate_Unauthorized()
        {
            var text = "Hello world (Storage Controller - Private)";
            var data = Encoding.UTF8.GetBytes(text);

            var props = new FileProperties
            {
                DocumentName = "Document N11",
                Description = "Document Description",
                Category = "Document Category",
                Subcategory = "Document Subcategory",
                Status = "Requested",
                Expiry = DateTimeOffset.UtcNow.AddDays(1),
                Received = DateTimeOffset.UtcNow.AddDays(-1),
                Alternated = DateTimeOffset.UtcNow,
            };

            FileStorageModel model;

            using (var file = new MemoryStream(data))
            {
                model = await _fixture.StorageService.CreateAsync(
                    file,
                    "Document File 11.txt",
                    Guid.Empty,
                    UserId1,
                    UserId1,
                    FileObjectType.User,
                    props,
                    new FileClaim[]
                    {
                        new FileClaim { ObjectIdentifier = GroupIdentifiers.PlatformAdministrator, ObjectType = FileClaimObjectType.Group }
                    }
                );
            }

            var relativeUrl = _fixture.StorageService.GetFileUrl(model.FileIdentifier, model.FileName);
            var absoluteUrl = $"{BaseAddress}/{relativeUrl}";

            var response = await StaticHttpClient.Client.GetAsync(absoluteUrl);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            await _fixture.StorageService.DeleteAsync(model.FileIdentifier);
        }

        [Fact]
        public async Task Files_StorageControllerPublic_WrongOrg()
        {
            var text = "Hello world (Storage Controller - Wrong Org)";
            var data = Encoding.UTF8.GetBytes(text);

            var props = new FileProperties
            {
                DocumentName = "Document N12",
                Description = "Document Description",
                Category = "Document Category",
                Subcategory = "Document Subcategory",
                Status = "Requested",
                Expiry = DateTimeOffset.UtcNow.AddDays(1),
                Received = DateTimeOffset.UtcNow.AddDays(-1),
                Alternated = DateTimeOffset.UtcNow,
            };

            FileStorageModel model;

            using (var file = new MemoryStream(data))
            {
                model = await _fixture.StorageService.CreateAsync(
                    file,
                    "Document File 12.txt",
                    GlobalId,
                    UserId1,
                    UserId1,
                    FileObjectType.User,
                    props,
                    null
                );
            }

            var relativeUrl = _fixture.StorageService.GetFileUrl(model.FileIdentifier, model.FileName);

            var absoluteUrl = $"{BaseAddress}/{relativeUrl}";

            var response = await StaticHttpClient.Client.GetAsync(absoluteUrl);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            await _fixture.StorageService.DeleteAsync(model.FileIdentifier);
        }

        [Fact]
        public async Task Files_TooLongFileNameTruncate_Success()
        {
            var fileName = "Feedback Victoria Principals' and Vice-Principals' Associaiton Principal Contract   with changes noted April 2015.docx  copy for Board of Education.docx";
            var data = Encoding.UTF8.GetBytes("Hello world");

            var props = new FileProperties
            {
                DocumentName = fileName,
                Description = "Document Description",
                Category = "Document Category",
                Subcategory = "Document Subcategory",
                Status = "Requested",
                Expiry = DateTimeOffset.UtcNow.AddDays(1),
                Received = DateTimeOffset.UtcNow.AddDays(-1),
                Alternated = DateTimeOffset.UtcNow,
            };

            FileStorageModel model;

            using (var file = new MemoryStream(data))
            {
                model = await _fixture.StorageService.CreateAsync(
                    file,
                    fileName,
                    GlobalId,
                    UserId1,
                    UserId1,
                    FileObjectType.User,
                    props,
                    null
                );
            }

            _fixture.StorageService.ClearCache();

            var model2 = await _fixture.StorageService.GetFileAsync(model.FileIdentifier);

            Assert.Equal(fileName, model2.Properties.DocumentName);
            Assert.True(fileName.Length > model2.FileName.Length);

            await _fixture.StorageService.DeleteAsync(model.FileIdentifier);
        }

        [Fact]
        public async Task Files_StorageControllerUploadTemp_Success()
        {
            var text = "Hello world (Storage Controller Upload)";
            var data = Encoding.UTF8.GetBytes(text);
            var fileName = "Document.txt";
            var surveySession = Guid.NewGuid();

            var create = new CreateResponseSession(surveySession, null, InSiteId, TestFixture.SurveyId, UserId1);

            create.OriginOrganization = InSiteId;

            create.OriginUser = UserId1;

            var client = new HttpClient();

            var tokenContent = JsonContent.Create(new JwtRequest { Secret = _fixture.Settings.Security.Sentinels.Root.Secret });

            var tokenResponse = await client.PostAsync($"{BaseAddress}/security/tokens/generate", tokenContent);

            var token = JsonConvert.DeserializeObject<JwtResponse>(await tokenResponse.Content.ReadAsStringAsync())!.AccessToken;

            _fixture.SendCommand(create);

            var absoluteUrl = $"{BaseAddress}/{StorageEndpoint}/temp?surveySession={surveySession}";

            UploadFileInfo[] files = [];

            using (var form = new MultipartFormDataContent())
            {
                form.Add(new ByteArrayContent(data, 0, data.Length), "File", fileName);

                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var response = await client.PostAsync(absoluteUrl, form);

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                files = JsonConvert.DeserializeObject<UploadFileInfo[]>(json)!;
            }

            Assert.Single(files);

            var (model, stream) = await _fixture.StorageService.GetFileStreamAsync(files[0].FileIdentifier);
            string actualText;

            using (var reader = new StreamReader(stream))
                actualText = await reader.ReadToEndAsync();

            Assert.Equal(text, actualText);
            Assert.Equal(fileName, model.FileName, ignoreCase: true);
            Assert.Equal(fileName, model.Properties.DocumentName);
            Assert.Equal(FileObjectType.Temporary, model.ObjectType);

            await _fixture.StorageService.DeleteAsync(model.FileIdentifier);
        }

        [Fact]
        public async Task Files_StorageControllerUploadTemp_Unauthorized()
        {
            var text = "Hello world (Storage Controller Upload)";
            var data = Encoding.UTF8.GetBytes(text);
            var fileName = "Document.txt";
            var absoluteUrl = $"{BaseAddress}/{StorageEndpoint}/temp";

            using (var form = new MultipartFormDataContent())
            {
                form.Add(new ByteArrayContent(data, 0, data.Length), "File", fileName);

                var response = await StaticHttpClient.Client.PostAsync(absoluteUrl, form);

                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }

        [Fact]
        public async Task Files_Rename_Success()
        {
            var text = "Hello world (Rename)";
            var data = Encoding.UTF8.GetBytes(text);

            var props = new FileProperties
            {
                DocumentName = "Document N8",
                Description = "Document Description",
                Category = "Document Category",
                Subcategory = "Document Subcategory",
                Status = "Requested",
                Expiry = DateTimeOffset.UtcNow.AddDays(1),
                Received = DateTimeOffset.UtcNow.AddDays(-1),
                Alternated = DateTimeOffset.UtcNow,
            };

            FileStorageModel model;

            using (var file = new MemoryStream(data))
            {
                model = await _fixture.StorageService.CreateAsync(
                    file,
                    "Document File 8.txt",
                    GlobalId,
                    UserId1,
                    UserId1,
                    FileObjectType.User,
                    props,
                    null
                );
            }

            const string newFileName = "New File Name.txt";

            await _fixture.StorageService.RenameFileAsync(model.FileIdentifier, UserId1, newFileName);

            var (model2, stream2) = await _fixture.StorageService.GetFileStreamAsync(model.FileIdentifier);
            var props2 = model2.Properties;

            string actualText;
            try
            {
                using (var reader = new StreamReader(stream2))
                    actualText = reader.ReadToEnd();
            }
            finally
            {
                stream2.Close();
            }

            Assert.Equal(text, actualText);

            Assert.NotEqual(model.FileName, model2.FileName);
            Assert.Equal(newFileName, props2.DocumentName);

            await _fixture.StorageService.DeleteAsync(model.FileIdentifier);
        }

        [Fact]
        public async Task Files_DeleteExpired_Success()
        {
            var text = "Hello world";
            var data = Encoding.UTF8.GetBytes(text);

            FileStorageModel model1, model2, model3;

            using (var file = new MemoryStream(data))
            {
                model1 = await _fixture.StorageService.CreateAsync(
                    file,
                    "Document File.txt",
                    GlobalId,
                    UserId1,
                    UserId1,
                    FileObjectType.User,
                    null,
                    null
                );

                file.Position = 0;

                model2 = await _fixture.StorageService.CreateAsync(
                    file,
                    "Document File.txt",
                    GlobalId,
                    UserId1,
                    UserId1,
                    FileObjectType.Temporary,
                    null,
                    null
                );

                file.Position = 0;

                model3 = await _fixture.StorageService.CreateAsync(
                    file,
                    "Document File.txt",
                    GlobalId,
                    UserId1,
                    UserId1,
                    FileObjectType.Temporary,
                    null,
                    null
                );
            }

            await _fixture.FileStore.UpdateFileUploadedAsync(model1.FileIdentifier, DateTimeOffset.UtcNow.AddMinutes(-10 - _fixture.Settings.Application.TempFileExpirationInMinutes));

            await _fixture.FileStore.UpdateFileUploadedAsync(model2.FileIdentifier, DateTimeOffset.UtcNow.AddMinutes(-10 - _fixture.Settings.Application.TempFileExpirationInMinutes));

            await _fixture.StorageService.DeleteExpiredFilesAsync();

            var model1_new = await _fixture.StorageService.GetFileAsync(model1.FileIdentifier);
            Assert.NotNull(model1_new);

            var model2_new = await _fixture.StorageService.GetFileAsync(model2.FileIdentifier);
            Assert.Null(model2_new);

            var model3_new = await _fixture.StorageService.GetFileAsync(model3.FileIdentifier);
            Assert.NotNull(model3_new);

            await _fixture.StorageService.DeleteAsync(model1.FileIdentifier);
            await _fixture.StorageService.DeleteAsync(model3.FileIdentifier);
        }

        [Fact]
        public void Files_ParseFileUrl_Success()
        {
            var fileId = Guid.NewGuid();
            var fileName = "this-is-a-test.txt";
            var url = _fixture.StorageService.GetFileUrl(fileId, fileName);

            var (parsedFileId, parsedFileName) = _fixture.StorageService.ParseFileUrl(url);

            Assert.Equal(fileId, parsedFileId);
            Assert.Equal(fileName, parsedFileName);
        }

        [Fact]
        public void Files_ParseFileUrl_Fail()
        {
            var (parsedFileId, parsedFileName) = _fixture.StorageService.ParseFileUrl("/in-content/surveys/48457/responses/375a6a4e-4f5c-412d-bb78-af1a010086d5/Document.pdf");

            Assert.Null(parsedFileId);
            Assert.Null(parsedFileName);
        }

        [Fact]
        public async Task Files_RemoteFile_Success()
        {
            if (!(_fixture.FileManagerService is FileManagerService fileManagerService))
                return;

            var randomId = Guid.NewGuid();
            var randomName = "remote-file.txt";
            var text = "Hello world";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
                await fileManagerService.SaveFileAsync(GlobalId, randomId, randomName, null, stream);

            var remoteFilePath = fileManagerService.GetFilePath(GlobalId, randomId, randomName);

            FileStorageModel model;

            try
            {
                var props = new FileProperties
                {
                    DocumentName = "Remote Document",
                };

                model = await _fixture.StorageService.CreateAsync(
                    null,
                    randomName,
                    GlobalId,
                    UserId1,
                    UserId1,
                    FileObjectType.User,
                    props,
                    null,
                    FileLocation.Remote,
                    remoteFilePath
                );

                var (model2, stream2) = await _fixture.StorageService.GetFileStreamAsync(model.FileIdentifier);

                string actualText;
                try
                {
                    using (var reader = new StreamReader(stream2))
                        actualText = reader.ReadToEnd();
                }
                finally
                {
                    stream2.Close();
                }

                Assert.Equal(FileLocation.Remote, model2.FileLocation);
                Assert.Equal(model.FileLocation, model2.FileLocation);
                Assert.Equal(Encoding.UTF8.GetBytes(text).Length, model2.FileSize);
                Assert.Equal(text, actualText);

                await _fixture.StorageService.DeleteAsync(model.FileIdentifier);

                Assert.True(_fixture.FileManagerService.IsFileExist(model.OrganizationIdentifier, model.FileIdentifier, model.FileName, model.FilePath));
            }
            finally
            {
                fileManagerService.DeleteFile(GlobalId, randomId, randomName);
            }

            Assert.False(_fixture.FileManagerService.IsFileExist(model.OrganizationIdentifier, model.FileIdentifier, model.FileName, model.FilePath));
        }

        [Fact]
        public async Task Files_ReadFileStream_Fail()
        {
            if (!(_fixture.FileManagerService is FileManagerService fileManagerService))
                return;

            var randomId = Guid.NewGuid();
            var randomName = "remote-file.txt";
            var text = "Hello world";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
                await fileManagerService.SaveFileAsync(GlobalId, randomId, randomName, null, stream);

            var remoteFilePath = fileManagerService.GetFilePath(GlobalId, randomId, randomName);

            var props = new FileProperties
            {
                DocumentName = "Remote Document",
            };

            var model = await _fixture.StorageService.CreateAsync(
                null,
                randomName,
                GlobalId,
                UserId1,
                UserId1,
                FileObjectType.User,
                props,
                null,
                FileLocation.Remote,
                remoteFilePath
            );

            fileManagerService.DeleteFile(GlobalId, randomId, randomName);

            await Assert.ThrowsAsync<ReadFileStreamFailedException>(() => _fixture.StorageService.GetFileStreamAsync(model.FileIdentifier));

            await _fixture.StorageService.DeleteAsync(model.FileIdentifier);
        }

        [Fact]
        public void Files_IsRemoteFilePathValid()
        {
            Assert.True(_fixture.StorageService.IsRemoteFilePathValid(new FileStorageModel { FileLocation = FileLocation.Local }));
            Assert.True(_fixture.StorageService.IsRemoteFilePathValid(new FileStorageModel { FileLocation = FileLocation.Remote, FilePath = @"C:\pathtofile" }));
            Assert.True(_fixture.StorageService.IsRemoteFilePathValid(new FileStorageModel { FileLocation = FileLocation.Remote, FilePath = @"\\pathtofile" }));
            Assert.True(_fixture.StorageService.IsRemoteFilePathValid(new FileStorageModel { FileLocation = FileLocation.Remote, FilePath = @"http://pathtofile" }));
            Assert.True(_fixture.StorageService.IsRemoteFilePathValid(new FileStorageModel { FileLocation = FileLocation.Remote, FilePath = @"https://pathtofile" }));
            Assert.False(_fixture.StorageService.IsRemoteFilePathValid(new FileStorageModel { FileLocation = FileLocation.Remote }));
            Assert.False(_fixture.StorageService.IsRemoteFilePathValid(new FileStorageModel { FileLocation = FileLocation.Remote, FilePath = "#" }));
            Assert.False(_fixture.StorageService.IsRemoteFilePathValid(new FileStorageModel { FileLocation = FileLocation.Remote, FilePath = @"ftp://pathtofile" }));
        }

        [Fact]
        public async Task Files_UnicodeFileName_Success()
        {
            var data = Encoding.UTF8.GetBytes("Hello world (Unicode FileName)");

            var props = new FileProperties
            {
                DocumentName = "Document N2"
            };

            FileStorageModel model;

            using (var file = new MemoryStream(data))
            {
                model = await _fixture.StorageService.CreateAsync(
                    file,
                    "certificatcăsătorieoriginal_кириллица.pdf",
                    GlobalId,
                    UserId2,
                    UserId2,
                    FileObjectType.User,
                    props,
                    null
                );
            }

            _fixture.StorageService.ClearCache();

            var (newModel, stream) = await _fixture.StorageService.GetFileStreamAsync(model.FileIdentifier);
            string actualText;

            using (var reader = new StreamReader(stream))
                actualText = await reader.ReadToEndAsync();

            await _fixture.StorageService.DeleteAsync(newModel.FileIdentifier);
        }
    }
}
