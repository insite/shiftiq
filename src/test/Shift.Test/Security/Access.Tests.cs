using Shift.Common;

namespace Shift.Test.Security;

public class FeatureAccessHelperTests
{
    [Fact]
    public void Constructor_SetsValue()
    {
        var helper = new FeatureAccessHelper(FeatureAccess.Trial);
        Assert.Equal(FeatureAccess.Trial, helper.Value);
    }

    [Fact]
    public void IsEmpty_WhenUnspecified_ReturnsTrue()
    {
        var helper = new FeatureAccessHelper(FeatureAccess.Unspecified);
        Assert.True(helper.IsEmpty);
    }

    [Fact]
    public void IsEmpty_WhenTrial_ReturnsFalse()
    {
        var helper = new FeatureAccessHelper(FeatureAccess.Trial);
        Assert.False(helper.IsEmpty);
    }

    [Fact]
    public void Use_WhenFeatureIsUse_ReturnsTrue()
    {
        var helper = new FeatureAccessHelper(FeatureAccess.Use);
        Assert.True(helper.Use);
    }

    [Fact]
    public void Use_WhenFeatureIsUnspecified_ReturnsFalse()
    {
        var helper = new FeatureAccessHelper(FeatureAccess.Unspecified);
        Assert.False(helper.Use);
    }

    [Fact]
    public void Abbreviate_WhenUse_ReturnsU()
    {
        var helper = new FeatureAccessHelper(FeatureAccess.Use);
        Assert.Equal("u", helper.Abbreviate());
    }

    [Fact]
    public void Abbreviate_WhenUnspecified_ReturnsDash()
    {
        var helper = new FeatureAccessHelper(FeatureAccess.Unspecified);
        Assert.Equal("-", helper.Abbreviate());
    }

    [Fact]
    public void Abbreviate_SingleValue_ReturnsCorrectAbbreviation()
    {
        var helper = new FeatureAccessHelper(FeatureAccess.Unspecified);
        Assert.Equal("u", helper.Abbreviate(FeatureAccess.Use));
    }

    [Fact]
    public void Describe_WhenUse_ReturnsUse()
    {
        var helper = new FeatureAccessHelper(FeatureAccess.Use);
        Assert.Equal("Use", helper.Describe());
    }

    [Fact]
    public void Describe_WhenUnspecified_ReturnsUnspecified()
    {
        var helper = new FeatureAccessHelper(FeatureAccess.Unspecified);
        Assert.Equal("Unspecified", helper.Describe());
    }

    [Fact]
    public void HasAny_WhenUse_ReturnsTrue()
    {
        var helper = new FeatureAccessHelper(FeatureAccess.Use);
        Assert.True(helper.HasAny());
    }

    [Fact]
    public void HasAny_WhenUnspecified_ReturnsFalse()
    {
        var helper = new FeatureAccessHelper(FeatureAccess.Unspecified);
        Assert.False(helper.HasAny());
    }

    [Fact]
    public void HasFlag_WhenFlagSet_ReturnsTrue()
    {
        var helper = new FeatureAccessHelper(FeatureAccess.Use);
        Assert.True(helper.HasFlag(FeatureAccess.Use));
    }

    [Fact]
    public void Add_FeatureValue_AddsFlag()
    {
        var helper = new FeatureAccessHelper(FeatureAccess.Unspecified);
        helper.Add(FeatureAccess.Use);
        Assert.True(helper.Use);
    }

    [Theory]
    [InlineData("use")]
    [InlineData("Use")]
    [InlineData("USE")]
    public void Add_StringValue_ParsesCaseInsensitively(string value)
    {
        var helper = new FeatureAccessHelper(FeatureAccess.Unspecified);
        helper.Add(new[] { value });
        Assert.True(helper.Use);
    }

    [Fact]
    public void Add_Wildcard_SetsUseFlag()
    {
        var helper = new FeatureAccessHelper(FeatureAccess.Unspecified);
        helper.Add(new[] { "*" });
        Assert.True(helper.Use);
    }

    [Fact]
    public void Add_AllKeyword_SetsUseFlag()
    {
        var helper = new FeatureAccessHelper(FeatureAccess.Unspecified);
        helper.Add(new[] { "all" });
        Assert.True(helper.Use);
    }

    [Fact]
    public void HasAll_WhenAllFlagsSet_ReturnsTrue()
    {
        var helper = new FeatureAccessHelper(FeatureAccess.Trial);
        helper.Add(FeatureAccess.Use);
        Assert.True(helper.HasAll());
    }

    [Fact]
    public void HasAll_WithStringValue_ChecksFlag()
    {
        var helper = new FeatureAccessHelper(FeatureAccess.Trial);
        helper.Add(FeatureAccess.Use);
        Assert.True(helper.HasAll("trial"));
    }
}

public class DataAccessHelperTests
{
    [Fact]
    public void Constructor_SetsValue()
    {
        var helper = new DataAccessHelper(DataAccess.Read);
        Assert.Equal(DataAccess.Read, helper.Value);
    }

    [Fact]
    public void IsEmpty_WhenUnspecified_ReturnsTrue()
    {
        var helper = new DataAccessHelper(DataAccess.Unspecified);
        Assert.True(helper.IsEmpty);
    }

    [Fact]
    public void IsEmpty_WhenHasFlags_ReturnsFalse()
    {
        var helper = new DataAccessHelper(DataAccess.Read);
        Assert.False(helper.IsEmpty);
    }

    [Theory]
    [InlineData(DataAccess.Read, true, false, false, false, false, false)]
    [InlineData(DataAccess.Update, false, true, false, false, false, false)]
    [InlineData(DataAccess.Create, false, false, true, false, false, false)]
    [InlineData(DataAccess.Delete, false, false, false, true, false, false)]
    [InlineData(DataAccess.Administrate, false, false, false, false, true, false)]
    [InlineData(DataAccess.Configure, false, false, false, false, false, true)]
    public void IndividualFlags_ReturnCorrectValues(
        DataAccess access, bool read, bool update, bool create, bool delete, bool admin, bool config)
    {
        var helper = new DataAccessHelper(access);
        Assert.Equal(read, helper.Read);
        Assert.Equal(update, helper.Update);
        Assert.Equal(create, helper.Create);
        Assert.Equal(delete, helper.Delete);
        Assert.Equal(admin, helper.Administrate);
        Assert.Equal(config, helper.Configure);
    }

    [Fact]
    public void CombinedFlags_AllReturnTrue()
    {
        var access = DataAccess.Read | DataAccess.Update | DataAccess.Delete;
        var helper = new DataAccessHelper(access);

        Assert.True(helper.Read);
        Assert.True(helper.Update);
        Assert.True(helper.Delete);
        Assert.False(helper.Create);
        Assert.False(helper.Administrate);
        Assert.False(helper.Configure);
    }

    [Fact]
    public void Abbreviate_SingleFlag_ReturnsCorrectAbbreviation()
    {
        var helper = new DataAccessHelper(DataAccess.Read);
        Assert.Equal("r", helper.Abbreviate());
    }

    [Fact]
    public void Abbreviate_MultipleFlags_ReturnsCombinedAbbreviations()
    {
        var helper = new DataAccessHelper(DataAccess.Read | DataAccess.Update);
        var abbrev = helper.Abbreviate();
        Assert.Contains("r", abbrev);
        Assert.Contains("u", abbrev);
    }

    [Fact]
    public void Abbreviate_Unspecified_ReturnsDash()
    {
        var helper = new DataAccessHelper(DataAccess.Unspecified);
        Assert.Equal("-", helper.Abbreviate());
    }

    [Theory]
    [InlineData(DataAccess.Read, "r")]
    [InlineData(DataAccess.Update, "u")]
    [InlineData(DataAccess.Create, "c")]
    [InlineData(DataAccess.Delete, "d")]
    [InlineData(DataAccess.Administrate, "a")]
    [InlineData(DataAccess.Configure, "f")]
    public void Abbreviate_SpecificValue_ReturnsCorrectAbbreviation(DataAccess access, string expected)
    {
        var helper = new DataAccessHelper(DataAccess.Unspecified);
        Assert.Equal(expected, helper.Abbreviate(access));
    }

    [Fact]
    public void Describe_SingleFlag_ReturnsName()
    {
        var helper = new DataAccessHelper(DataAccess.Read);
        Assert.Equal("Read", helper.Describe());
    }

    [Fact]
    public void Describe_MultipleFlags_ReturnsCommaSeparatedNames()
    {
        var helper = new DataAccessHelper(DataAccess.Read | DataAccess.Update);
        var desc = helper.Describe();
        Assert.Contains("Read", desc);
        Assert.Contains("Update", desc);
        Assert.Contains(",", desc);
    }

    [Fact]
    public void HasAny_WithFlags_ReturnsTrue()
    {
        var helper = new DataAccessHelper(DataAccess.Read);
        Assert.True(helper.HasAny());
    }

    [Fact]
    public void HasAny_WithUnspecified_ReturnsFalse()
    {
        var helper = new DataAccessHelper(DataAccess.Unspecified);
        Assert.False(helper.HasAny());
    }

    [Fact]
    public void HasAny_WithOverlappingFlags_ReturnsTrue()
    {
        var helper = new DataAccessHelper(DataAccess.Read | DataAccess.Update);
        Assert.True(helper.HasAny(DataAccess.Read));
    }

    [Fact]
    public void HasAny_WithNonOverlappingFlags_ReturnsFalse()
    {
        var helper = new DataAccessHelper(DataAccess.Read);
        Assert.False(helper.HasAny(DataAccess.Update));
    }

    [Fact]
    public void HasFlag_WhenFlagPresent_ReturnsTrue()
    {
        var helper = new DataAccessHelper(DataAccess.Read | DataAccess.Update);
        Assert.True(helper.HasFlag(DataAccess.Read));
    }

    [Fact]
    public void HasFlag_WhenFlagMissing_ReturnsFalse()
    {
        var helper = new DataAccessHelper(DataAccess.Read);
        Assert.False(helper.HasFlag(DataAccess.Update));
    }

    [Fact]
    public void Add_SingleFlag_AddsToValue()
    {
        var helper = new DataAccessHelper(DataAccess.Unspecified);
        helper.Add(DataAccess.Read);
        Assert.True(helper.Read);
    }

    [Fact]
    public void Add_MultipleFlags_AccumulatesFlags()
    {
        var helper = new DataAccessHelper(DataAccess.Read);
        helper.Add(DataAccess.Update);
        Assert.True(helper.Read);
        Assert.True(helper.Update);
    }

    [Theory]
    [InlineData("read", DataAccess.Read)]
    [InlineData("Read", DataAccess.Read)]
    [InlineData("READ", DataAccess.Read)]
    [InlineData("update", DataAccess.Update)]
    [InlineData("create", DataAccess.Create)]
    [InlineData("delete", DataAccess.Delete)]
    [InlineData("administrate", DataAccess.Administrate)]
    [InlineData("configure", DataAccess.Configure)]
    public void Add_StringArray_ParsesCorrectly(string input, DataAccess expected)
    {
        var helper = new DataAccessHelper(DataAccess.Unspecified);
        helper.Add(new[] { input });
        Assert.True(helper.HasFlag(expected));
    }

    [Fact]
    public void Add_MultipleStrings_AddsAllFlags()
    {
        var helper = new DataAccessHelper(DataAccess.Unspecified);
        helper.Add(new[] { "read", "update", "delete" });

        Assert.True(helper.Read);
        Assert.True(helper.Update);
        Assert.True(helper.Delete);
        Assert.False(helper.Create);
    }

    [Fact]
    public void Add_Wildcard_SetsAllFlags()
    {
        var helper = new DataAccessHelper(DataAccess.Unspecified);
        helper.Add(new[] { "*" });

        Assert.True(helper.Read);
        Assert.True(helper.Update);
        Assert.True(helper.Create);
        Assert.True(helper.Delete);
        Assert.True(helper.Administrate);
        Assert.True(helper.Configure);
    }

    [Fact]
    public void Add_AllKeyword_SetsAllFlags()
    {
        var helper = new DataAccessHelper(DataAccess.Unspecified);
        helper.Add(new[] { "ALL" });
        Assert.True(helper.HasAll());
    }

    [Fact]
    public void Add_InvalidString_DoesNotThrow()
    {
        var helper = new DataAccessHelper(DataAccess.Unspecified);
        helper.Add(new[] { "invalid" });
        Assert.Equal(DataAccess.Unspecified, helper.Value);
    }

    [Fact]
    public void HasAll_WhenAllFlagsSet_ReturnsTrue()
    {
        var all = DataAccess.Read | DataAccess.Update | DataAccess.Create |
                  DataAccess.Delete | DataAccess.Administrate | DataAccess.Configure;
        var helper = new DataAccessHelper(all);
        Assert.True(helper.HasAll());
    }

    [Fact]
    public void HasAll_WhenSomeFlagsMissing_ReturnsFalse()
    {
        var helper = new DataAccessHelper(DataAccess.Read | DataAccess.Update);
        Assert.False(helper.HasAll());
    }

    [Fact]
    public void HasAll_StringVersion_ChecksSpecificFlag()
    {
        var helper = new DataAccessHelper(DataAccess.Read | DataAccess.Update);
        Assert.True(helper.HasAll("read"));
        Assert.False(helper.HasAll("delete"));
    }
}

public class AuthorityAccessHelperTests
{
    [Fact]
    public void Constructor_SetsValue()
    {
        var helper = new AuthorityAccessHelper(AuthorityAccess.Administrator);
        Assert.Equal(AuthorityAccess.Administrator, helper.Value);
    }

    [Fact]
    public void IsEmpty_WhenUnspecified_ReturnsTrue()
    {
        var helper = new AuthorityAccessHelper(AuthorityAccess.Unspecified);
        Assert.True(helper.IsEmpty);
    }

    [Fact]
    public void AllIndividualFlags_WorkCorrectly()
    {
        Assert.True(new AuthorityAccessHelper(AuthorityAccess.Guest).Guest);
        Assert.True(new AuthorityAccessHelper(AuthorityAccess.Member).Member);
        Assert.True(new AuthorityAccessHelper(AuthorityAccess.Trainee).Trainee);
        Assert.True(new AuthorityAccessHelper(AuthorityAccess.Learner).Learner);
        Assert.True(new AuthorityAccessHelper(AuthorityAccess.Instructor).Instructor);
        Assert.True(new AuthorityAccessHelper(AuthorityAccess.Validator).Validator);
        Assert.True(new AuthorityAccessHelper(AuthorityAccess.Supervisor).Supervisor);
        Assert.True(new AuthorityAccessHelper(AuthorityAccess.Manager).Manager);
        Assert.True(new AuthorityAccessHelper(AuthorityAccess.Administrator).Administrator);
        Assert.True(new AuthorityAccessHelper(AuthorityAccess.Developer).Developer);
        Assert.True(new AuthorityAccessHelper(AuthorityAccess.Operator).Operator);
    }

    [Fact]
    public void CombinedFlags_WorkCorrectly()
    {
        var access = AuthorityAccess.Administrator | AuthorityAccess.Developer | AuthorityAccess.Operator;
        var helper = new AuthorityAccessHelper(access);

        Assert.False(helper.Guest);
        Assert.False(helper.Member);
        Assert.True(helper.Administrator);
        Assert.True(helper.Developer);
        Assert.True(helper.Operator);
    }

    [Theory]
    [InlineData(AuthorityAccess.Guest, "g")]
    [InlineData(AuthorityAccess.Member, "b")]
    [InlineData(AuthorityAccess.Trainee, "t")]
    [InlineData(AuthorityAccess.Learner, "l")]
    [InlineData(AuthorityAccess.Instructor, "i")]
    [InlineData(AuthorityAccess.Validator, "v")]
    [InlineData(AuthorityAccess.Supervisor, "s")]
    [InlineData(AuthorityAccess.Manager, "m")]
    [InlineData(AuthorityAccess.Administrator, "a")]
    [InlineData(AuthorityAccess.Developer, "d")]
    [InlineData(AuthorityAccess.Operator, "o")]
    public void Abbreviate_SpecificValue_ReturnsCorrectAbbreviation(AuthorityAccess access, string expected)
    {
        var helper = new AuthorityAccessHelper(AuthorityAccess.Unspecified);
        Assert.Equal(expected, helper.Abbreviate(access));
    }

    [Theory]
    [InlineData("guest", AuthorityAccess.Guest)]
    [InlineData("Administrator", AuthorityAccess.Administrator)]
    [InlineData("DEVELOPER", AuthorityAccess.Developer)]
    public void Add_StringArray_ParsesCorrectly(string input, AuthorityAccess expected)
    {
        var helper = new AuthorityAccessHelper(AuthorityAccess.Unspecified);
        helper.Add(new[] { input });
        Assert.True(helper.HasFlag(expected));
    }

    [Fact]
    public void Add_Wildcard_SetsAllFlags()
    {
        var helper = new AuthorityAccessHelper(AuthorityAccess.Unspecified);
        helper.Add(new[] { "*" });

        Assert.True(helper.Guest);
        Assert.True(helper.Member);
        Assert.True(helper.Trainee);
        Assert.True(helper.Learner);
        Assert.True(helper.Instructor);
        Assert.True(helper.Validator);
        Assert.True(helper.Supervisor);
        Assert.True(helper.Manager);
        Assert.True(helper.Administrator);
        Assert.True(helper.Developer);
        Assert.True(helper.Operator);
    }

    [Fact]
    public void HasAll_WhenAllFlagsSet_ReturnsTrue()
    {
        var all = AuthorityAccess.Guest | AuthorityAccess.Member | AuthorityAccess.Trainee |
                  AuthorityAccess.Learner | AuthorityAccess.Instructor | AuthorityAccess.Validator |
                  AuthorityAccess.Supervisor | AuthorityAccess.Manager | AuthorityAccess.Administrator |
                  AuthorityAccess.Developer | AuthorityAccess.Operator;
        var helper = new AuthorityAccessHelper(all);
        Assert.True(helper.HasAll());
    }
}

public class AccessTests
{
    [Fact]
    public void Add_FormalFeaturePattern_ParsesCorrectly()
    {
        var access = new AccessSet();
        access.Add("feature:trial,use");
        Assert.True(access.Feature.HasFlag(FeatureAccess.Trial));
        Assert.True(access.Feature.HasFlag(FeatureAccess.Use));
    }

    [Fact]
    public void Add_FormalDataPattern_ParsesCorrectly()
    {
        var access = new AccessSet();
        access.Add("data:read,update");
        Assert.True(access.Data.HasFlag(DataAccess.Read));
        Assert.True(access.Data.HasFlag(DataAccess.Update));
    }

    [Fact]
    public void Add_FormalAuthorityPattern_ParsesCorrectly()
    {
        var access = new AccessSet();
        access.Add("authority:administrator,developer");
        Assert.True(access.Authority.HasFlag(AuthorityAccess.Administrator));
        Assert.True(access.Authority.HasFlag(AuthorityAccess.Developer));
    }

    [Fact]
    public void Add_WildcardData_SetsAllFlags()
    {
        var access = new AccessSet();
        access.Add("data:*");
        Assert.True(access.Data.HasFlag(DataAccess.Read));
        Assert.True(access.Data.HasFlag(DataAccess.Update));
        Assert.True(access.Data.HasFlag(DataAccess.Create));
        Assert.True(access.Data.HasFlag(DataAccess.Delete));
        Assert.True(access.Data.HasFlag(DataAccess.Administrate));
        Assert.True(access.Data.HasFlag(DataAccess.Configure));
    }

    [Theory]
    [InlineData("use")]
    [InlineData("Use")]
    [InlineData("USE")]
    public void Add_SimplePattern_SetsFeatureUse(string input)
    {
        var access = new AccessSet();
        access.Add(input);
        Assert.Equal(FeatureAccess.Use, access.Feature);
    }

    [Fact]
    public void Add_CaseInsensitive_ParsesCorrectly()
    {
        var access = new AccessSet();
        access.Add("DATA:READ,UPDATE");
        Assert.True(access.Data.HasFlag(DataAccess.Read));
        Assert.True(access.Data.HasFlag(DataAccess.Update));
    }

    [Fact]
    public void Add_InvalidPattern_DoesNotThrow()
    {
        var access = new AccessSet();
        access.Add("invalid:pattern:here");
        Assert.Equal(FeatureAccess.Unspecified, access.Feature);
        Assert.Equal(DataAccess.Unspecified, access.Data);
    }

    [Fact]
    public void Add_AnotherAccessObject_CombinesFlags()
    {
        var access1 = new AccessSet();
        access1.Add("data:read");

        var access2 = new AccessSet();
        access2.Add("data:update");

        access1.Add(access2);

        Assert.True(access1.Data.HasFlag(DataAccess.Read));
        Assert.True(access1.Data.HasFlag(DataAccess.Update));
    }

    [Fact]
    public void HasAny_WhenEmpty_ReturnsFalse()
    {
        var access = new AccessSet();
        Assert.False(access.HasAny());
    }

    [Fact]
    public void HasAny_WhenFeatureTrial_ReturnsTrue()
    {
        var access = new AccessSet();
        access.Add("feature:trial");
        Assert.True(access.HasAny());
    }

    [Fact]
    public void HasAny_WhenDataSet_ReturnsTrue()
    {
        var access = new AccessSet();
        access.Add("data:read");
        Assert.True(access.HasAny());
    }

    [Fact]
    public void HasAny_WhenAuthoritySet_ReturnsTrue()
    {
        var access = new AccessSet();
        access.Add("authority:administrator");
        Assert.True(access.HasAny());
    }

    [Theory]
    [InlineData(FeatureAccess.Use, true)]
    [InlineData(FeatureAccess.Trial, true)]
    public void Has_FeatureAccess_ReturnsCorrectValue(FeatureAccess check, bool expected)
    {
        var access = new AccessSet();
        access.Add("feature:trial,use");
        Assert.Equal(expected, access.Has(check));
    }

    [Fact]
    public void Has_DataAccess_ReturnsCorrectValue()
    {
        var access = new AccessSet();
        access.Add("data:read,update");
        Assert.True(access.Has(DataAccess.Read));
        Assert.True(access.Has(DataAccess.Update));
        Assert.False(access.Has(DataAccess.Delete));
    }

    [Fact]
    public void Has_AuthorityAccess_ReturnsCorrectValue()
    {
        var access = new AccessSet();
        access.Add("authority:administrator");
        Assert.True(access.Has(AuthorityAccess.Administrator));
        Assert.False(access.Has(AuthorityAccess.Developer));
    }
}

public class AccessControlTests
{
    [Fact]
    public void Constructor_InitializesGrantedAndDenied()
    {
        var control = new AccessControl();
        Assert.NotNull(control.Granted);
        Assert.NotNull(control.Denied);
    }

    [Fact]
    public void Add_AllowString_AddsToGranted()
    {
        var control = new AccessControl();
        control.Add("allow");
        Assert.True(control.Granted.Feature.HasFlag(FeatureAccess.Trial));
        Assert.True(control.Granted.Feature.HasFlag(FeatureAccess.Use));
    }

    [Fact]
    public void Add_AllowWithAccess_AddsToGranted()
    {
        var control = new AccessControl();
        control.Add("allow:data:read");
        Assert.True(control.Granted.Data.HasFlag(DataAccess.Read));
    }

    [Fact]
    public void Add_DenyString_AddsToDenied()
    {
        var control = new AccessControl();
        control.Add("deny:feature:use");
        Assert.Equal(FeatureAccess.Use, control.Denied.Feature);
    }

    [Fact]
    public void Add_DenyWithAccess_AddsToDenied()
    {
        var control = new AccessControl();
        control.Add("deny:data:delete");
        Assert.True(control.Denied.Data.HasFlag(DataAccess.Delete));
    }

    [Fact]
    public void Add_CaseInsensitive_ParsesCorrectly()
    {
        var control = new AccessControl();
        control.Add("ALLOW:DATA:READ");
        Assert.True(control.Granted.Data.HasFlag(DataAccess.Read));
    }

    [Fact]
    public void Add_InvalidPattern_DoesNotThrow()
    {
        var control = new AccessControl();
        control.Add("invalid");
        Assert.False(control.Granted.HasAny());
        Assert.False(control.Denied.HasAny());
    }

    [Fact]
    public void Add_AnotherAccessControl_CombinesBoth()
    {
        var control1 = new AccessControl();
        control1.Add("allow:data:read");

        var control2 = new AccessControl();
        control2.Add("allow:data:update");
        control2.Add("deny:data:delete");

        control1.Add(control2);

        Assert.True(control1.Granted.Data.HasFlag(DataAccess.Read));
        Assert.True(control1.Granted.Data.HasFlag(DataAccess.Update));
        Assert.True(control1.Denied.Data.HasFlag(DataAccess.Delete));
    }

    [Fact]
    public void Grant_FeatureAccess_AddsToGranted()
    {
        var control = new AccessControl();
        control.Grant(FeatureAccess.Use);
        Assert.True(control.IsGranted(FeatureAccess.Use));
    }

    [Fact]
    public void Deny_FeatureAccess_AddsToDenied()
    {
        var control = new AccessControl();
        control.Deny(FeatureAccess.Use);
        Assert.True(control.IsDenied(FeatureAccess.Use));
    }

    [Fact]
    public void Grant_DataAccess_AddsToGranted()
    {
        var control = new AccessControl();
        control.Grant(DataAccess.Read);
        Assert.True(control.IsGranted(DataAccess.Read));
    }

    [Fact]
    public void Deny_DataAccess_AddsToDenied()
    {
        var control = new AccessControl();
        control.Deny(DataAccess.Delete);
        Assert.True(control.IsDenied(DataAccess.Delete));
    }

    [Fact]
    public void IsAllowed_NoGrants_ReturnsFalse()
    {
        var control = new AccessControl();
        Assert.False(control.IsAllowed());
    }

    [Fact]
    public void IsAllowed_WithGrants_ReturnsTrue()
    {
        var control = new AccessControl();
        control.Grant(FeatureAccess.Use);
        Assert.True(control.IsAllowed());
    }

    [Fact]
    public void IsAllowed_FeatureAccess_WhenGrantedNotDenied_ReturnsTrue()
    {
        var control = new AccessControl();
        control.Grant(FeatureAccess.Use);
        Assert.True(control.IsAllowed(FeatureAccess.Use));
    }

    [Fact]
    public void IsAllowed_FeatureAccess_WhenDenied_ReturnsFalse()
    {
        var control = new AccessControl();
        control.Grant(FeatureAccess.Use);
        control.Deny(FeatureAccess.Use);
        Assert.False(control.IsAllowed(FeatureAccess.Use));
    }

    [Fact]
    public void IsAllowed_FeatureAccess_WhenNotGranted_ReturnsFalse()
    {
        var control = new AccessControl();
        Assert.False(control.IsAllowed(FeatureAccess.Use));
    }

    [Fact]
    public void IsAllowed_DataAccess_WhenGrantedNotDenied_ReturnsTrue()
    {
        var control = new AccessControl();
        control.Grant(DataAccess.Read);
        Assert.True(control.IsAllowed(DataAccess.Read));
    }

    [Fact]
    public void IsAllowed_DataAccess_WhenDenied_ReturnsFalse()
    {
        var control = new AccessControl();
        control.Grant(DataAccess.Read);
        control.Deny(DataAccess.Read);
        Assert.False(control.IsAllowed(DataAccess.Read));
    }

    [Fact]
    public void IsAllowed_AuthorityAccess_WhenGrantedNotDenied_ReturnsTrue()
    {
        var control = new AccessControl();
        control.Granted.Add("authority:administrator");
        Assert.True(control.IsAllowed(AuthorityAccess.Administrator));
    }

    [Fact]
    public void IsAllowed_AuthorityAccess_WhenDenied_ReturnsFalse()
    {
        var control = new AccessControl();
        control.Granted.Add("authority:administrator");
        control.Denied.Add("authority:administrator");
        Assert.False(control.IsAllowed(AuthorityAccess.Administrator));
    }

    [Fact]
    public void IsGranted_FeatureAccess_WhenGranted_ReturnsTrue()
    {
        var control = new AccessControl();
        control.Grant(FeatureAccess.Use);
        Assert.True(control.IsGranted(FeatureAccess.Use));
    }

    [Fact]
    public void IsGranted_FeatureAccess_WhenNotGranted_ReturnsFalse()
    {
        var control = new AccessControl();
        Assert.False(control.IsGranted(FeatureAccess.Use));
    }

    [Fact]
    public void IsDenied_FeatureAccess_WhenDenied_ReturnsTrue()
    {
        var control = new AccessControl();
        control.Deny(FeatureAccess.Use);
        Assert.True(control.IsDenied(FeatureAccess.Use));
    }

    [Fact]
    public void IsDenied_FeatureAccess_WhenNotDenied_ReturnsFalse()
    {
        var control = new AccessControl();
        Assert.False(control.IsDenied(FeatureAccess.Use));
    }

    [Fact]
    public void IsGranted_DataAccess_WhenGranted_ReturnsTrue()
    {
        var control = new AccessControl();
        control.Grant(DataAccess.Read);
        Assert.True(control.IsGranted(DataAccess.Read));
    }

    [Fact]
    public void IsDenied_DataAccess_WhenDenied_ReturnsTrue()
    {
        var control = new AccessControl();
        control.Deny(DataAccess.Delete);
        Assert.True(control.IsDenied(DataAccess.Delete));
    }

    [Fact]
    public void IsGranted_AuthorityAccess_WhenGranted_ReturnsTrue()
    {
        var control = new AccessControl();
        control.Granted.Add("authority:developer");
        Assert.True(control.IsGranted(AuthorityAccess.Developer));
    }

    [Fact]
    public void IsDenied_AuthorityAccess_WhenDenied_ReturnsTrue()
    {
        var control = new AccessControl();
        control.Denied.Add("authority:guest");
        Assert.True(control.IsDenied(AuthorityAccess.Guest));
    }

    [Fact]
    public void DenyTakesPrecedence_OverGrant()
    {
        var control = new AccessControl();
        control.Grant(DataAccess.Read);
        control.Grant(DataAccess.Update);
        control.Grant(DataAccess.Delete);
        control.Deny(DataAccess.Delete);

        Assert.True(control.IsAllowed(DataAccess.Read));
        Assert.True(control.IsAllowed(DataAccess.Update));
        Assert.False(control.IsAllowed(DataAccess.Delete));
    }

    [Fact]
    public void ComplexScenario_MultipleAccessTypes()
    {
        var control = new AccessControl();

        control.Add("allow:feature:trial");
        control.Add("allow:data:read,update,create");
        control.Add("deny:data:delete");
        control.Add("allow:authority:administrator,developer");

        Assert.True(control.IsAllowed(FeatureAccess.Trial));
        Assert.True(control.IsAllowed(DataAccess.Read));
        Assert.True(control.IsAllowed(DataAccess.Update));
        Assert.True(control.IsAllowed(DataAccess.Create));
        Assert.False(control.IsAllowed(DataAccess.Delete));
        Assert.True(control.IsAllowed(AuthorityAccess.Administrator));
        Assert.True(control.IsAllowed(AuthorityAccess.Developer));
        Assert.False(control.IsAllowed(AuthorityAccess.Operator));
    }
}

public class EnumFlagsTests
{
    [Fact]
    public void DataAccess_FlagsArePowersOfTwo()
    {
        Assert.Equal(0, (int)DataAccess.Unspecified);
        Assert.Equal(1, (int)DataAccess.Read);
        Assert.Equal(2, (int)DataAccess.Update);
        Assert.Equal(4, (int)DataAccess.Create);
        Assert.Equal(8, (int)DataAccess.Delete);
        Assert.Equal(16, (int)DataAccess.Administrate);
        Assert.Equal(32, (int)DataAccess.Configure);
    }

    [Fact]
    public void AuthorityAccess_FlagsArePowersOfTwo()
    {
        Assert.Equal(0, (int)AuthorityAccess.Unspecified);
        Assert.Equal(1, (int)AuthorityAccess.Guest);
        Assert.Equal(2, (int)AuthorityAccess.Member);
        Assert.Equal(4, (int)AuthorityAccess.Trainee);
        Assert.Equal(8, (int)AuthorityAccess.Learner);
        Assert.Equal(16, (int)AuthorityAccess.Instructor);
        Assert.Equal(32, (int)AuthorityAccess.Validator);
        Assert.Equal(64, (int)AuthorityAccess.Supervisor);
        Assert.Equal(128, (int)AuthorityAccess.Manager);
        Assert.Equal(256, (int)AuthorityAccess.Administrator);
        Assert.Equal(512, (int)AuthorityAccess.Developer);
        Assert.Equal(1024, (int)AuthorityAccess.Operator);
    }

    [Fact]
    public void FeatureAccess_FlagsAreCorrect()
    {
        Assert.Equal(0, (int)FeatureAccess.Unspecified);
        Assert.Equal(1, (int)FeatureAccess.Trial);
        Assert.Equal(2, (int)FeatureAccess.Use);
    }

    [Fact]
    public void CombinedFlags_WorkWithBitwiseOr()
    {
        var combined = DataAccess.Read | DataAccess.Update;
        Assert.Equal(3, (int)combined);
    }

    [Fact]
    public void FlagsCanBeCheckedWithHasFlag()
    {
        var combined = DataAccess.Read | DataAccess.Update | DataAccess.Delete;
        Assert.True(combined.HasFlag(DataAccess.Read));
        Assert.True(combined.HasFlag(DataAccess.Update));
        Assert.True(combined.HasFlag(DataAccess.Delete));
        Assert.False(combined.HasFlag(DataAccess.Create));
    }
}

public class AccessTypeEnumTests
{
    [Theory]
    [InlineData("Authority", AccessType.Authority)]
    [InlineData("Data", AccessType.Data)]
    [InlineData("Feature", AccessType.Feature)]
    public void AccessType_ParsesCorrectly(string input, AccessType expected)
    {
        var result = Enum.Parse<AccessType>(input, ignoreCase: true);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void AccessType_HasExpectedValues()
    {
        var values = Enum.GetValues<AccessType>();
        Assert.Equal(3, values.Length);
        Assert.Contains(AccessType.Authority, values);
        Assert.Contains(AccessType.Data, values);
        Assert.Contains(AccessType.Feature, values);
    }
}