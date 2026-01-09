using Shift.Common;

namespace Shift.Test.Security;

public class SwitchAccessHelperTests
{
    [Fact]
    public void Constructor_SetsValue()
    {
        var helper = new SwitchAccessHelper(SwitchAccess.On);
        Assert.Equal(SwitchAccess.On, helper.Value);
    }

    [Fact]
    public void IsEmpty_WhenOff_ReturnsTrue()
    {
        var helper = new SwitchAccessHelper(SwitchAccess.Off);
        Assert.True(helper.IsEmpty);
    }

    [Fact]
    public void IsEmpty_WhenOn_ReturnsFalse()
    {
        var helper = new SwitchAccessHelper(SwitchAccess.On);
        Assert.False(helper.IsEmpty);
    }

    [Fact]
    public void On_WhenSwitchIsOn_ReturnsTrue()
    {
        var helper = new SwitchAccessHelper(SwitchAccess.On);
        Assert.True(helper.On);
    }

    [Fact]
    public void On_WhenSwitchIsOff_ReturnsFalse()
    {
        var helper = new SwitchAccessHelper(SwitchAccess.Off);
        Assert.False(helper.On);
    }

    [Fact]
    public void Abbreviate_WhenOn_ReturnsPlus()
    {
        var helper = new SwitchAccessHelper(SwitchAccess.On);
        Assert.Equal("+", helper.Abbreviate());
    }

    [Fact]
    public void Abbreviate_WhenOff_ReturnsDash()
    {
        var helper = new SwitchAccessHelper(SwitchAccess.Off);
        Assert.Equal("-", helper.Abbreviate());
    }

    [Fact]
    public void Abbreviate_SingleValue_ReturnsCorrectAbbreviation()
    {
        var helper = new SwitchAccessHelper(SwitchAccess.Off);
        Assert.Equal("+", helper.Abbreviate(SwitchAccess.On));
    }

    [Fact]
    public void Describe_WhenOn_ReturnsOn()
    {
        var helper = new SwitchAccessHelper(SwitchAccess.On);
        Assert.Equal("On", helper.Describe());
    }

    [Fact]
    public void Describe_WhenOff_ReturnsOff()
    {
        var helper = new SwitchAccessHelper(SwitchAccess.Off);
        Assert.Equal("Off", helper.Describe());
    }

    [Fact]
    public void HasAny_WhenOn_ReturnsTrue()
    {
        var helper = new SwitchAccessHelper(SwitchAccess.On);
        Assert.True(helper.HasAny());
    }

    [Fact]
    public void HasAny_WhenOff_ReturnsFalse()
    {
        var helper = new SwitchAccessHelper(SwitchAccess.Off);
        Assert.False(helper.HasAny());
    }

    [Fact]
    public void HasFlag_WhenFlagSet_ReturnsTrue()
    {
        var helper = new SwitchAccessHelper(SwitchAccess.On);
        Assert.True(helper.HasFlag(SwitchAccess.On));
    }

    [Fact]
    public void Add_SwitchValue_AddsFlag()
    {
        var helper = new SwitchAccessHelper(SwitchAccess.Off);
        helper.Add(SwitchAccess.On);
        Assert.True(helper.On);
    }

    [Theory]
    [InlineData("on")]
    [InlineData("On")]
    [InlineData("ON")]
    public void Add_StringValue_ParsesCaseInsensitively(string value)
    {
        var helper = new SwitchAccessHelper(SwitchAccess.Off);
        helper.Add(new[] { value });
        Assert.True(helper.On);
    }

    [Fact]
    public void Add_Wildcard_SetsAllFlags()
    {
        var helper = new SwitchAccessHelper(SwitchAccess.Off);
        helper.Add(new[] { "*" });
        Assert.True(helper.On);
    }

    [Fact]
    public void Add_AllKeyword_SetsAllFlags()
    {
        var helper = new SwitchAccessHelper(SwitchAccess.Off);
        helper.Add(new[] { "all" });
        Assert.True(helper.On);
    }

    [Fact]
    public void HasAll_WhenAllFlagsSet_ReturnsTrue()
    {
        var helper = new SwitchAccessHelper(SwitchAccess.On);
        Assert.True(helper.HasAll());
    }

    [Fact]
    public void HasAll_WithStringValue_ChecksFlag()
    {
        var helper = new SwitchAccessHelper(SwitchAccess.On);
        Assert.True(helper.HasAll("on"));
    }
}

public class OperationAccessHelperTests
{
    [Fact]
    public void Constructor_SetsValue()
    {
        var helper = new OperationAccessHelper(OperationAccess.Read);
        Assert.Equal(OperationAccess.Read, helper.Value);
    }

    [Fact]
    public void IsEmpty_WhenNone_ReturnsTrue()
    {
        var helper = new OperationAccessHelper(OperationAccess.None);
        Assert.True(helper.IsEmpty);
    }

    [Fact]
    public void IsEmpty_WhenHasFlags_ReturnsFalse()
    {
        var helper = new OperationAccessHelper(OperationAccess.Read);
        Assert.False(helper.IsEmpty);
    }

    [Theory]
    [InlineData(OperationAccess.Read, true, false, false, false, false, false)]
    [InlineData(OperationAccess.Write, false, true, false, false, false, false)]
    [InlineData(OperationAccess.Create, false, false, true, false, false, false)]
    [InlineData(OperationAccess.Delete, false, false, false, true, false, false)]
    [InlineData(OperationAccess.Administrate, false, false, false, false, true, false)]
    [InlineData(OperationAccess.Configure, false, false, false, false, false, true)]
    public void IndividualFlags_ReturnCorrectValues(
        OperationAccess access, bool read, bool write, bool create, bool delete, bool admin, bool config)
    {
        var helper = new OperationAccessHelper(access);
        Assert.Equal(read, helper.Read);
        Assert.Equal(write, helper.Write);
        Assert.Equal(create, helper.Create);
        Assert.Equal(delete, helper.Delete);
        Assert.Equal(admin, helper.Administrate);
        Assert.Equal(config, helper.Configure);
    }

    [Fact]
    public void CombinedFlags_AllReturnTrue()
    {
        var access = OperationAccess.Read | OperationAccess.Write | OperationAccess.Delete;
        var helper = new OperationAccessHelper(access);

        Assert.True(helper.Read);
        Assert.True(helper.Write);
        Assert.True(helper.Delete);
        Assert.False(helper.Create);
        Assert.False(helper.Administrate);
        Assert.False(helper.Configure);
    }

    [Fact]
    public void Abbreviate_SingleFlag_ReturnsCorrectAbbreviation()
    {
        var helper = new OperationAccessHelper(OperationAccess.Read);
        Assert.Equal("r", helper.Abbreviate());
    }

    [Fact]
    public void Abbreviate_MultipleFlags_ReturnsCombinedAbbreviations()
    {
        var helper = new OperationAccessHelper(OperationAccess.Read | OperationAccess.Write);
        var abbrev = helper.Abbreviate();
        Assert.Contains("r", abbrev);
        Assert.Contains("w", abbrev);
    }

    [Fact]
    public void Abbreviate_None_ReturnsDash()
    {
        var helper = new OperationAccessHelper(OperationAccess.None);
        Assert.Equal("-", helper.Abbreviate());
    }

    [Theory]
    [InlineData(OperationAccess.Read, "r")]
    [InlineData(OperationAccess.Write, "w")]
    [InlineData(OperationAccess.Create, "c")]
    [InlineData(OperationAccess.Delete, "d")]
    [InlineData(OperationAccess.Administrate, "a")]
    [InlineData(OperationAccess.Configure, "f")]
    public void Abbreviate_SpecificValue_ReturnsCorrectAbbreviation(OperationAccess access, string expected)
    {
        var helper = new OperationAccessHelper(OperationAccess.None);
        Assert.Equal(expected, helper.Abbreviate(access));
    }

    [Fact]
    public void Describe_SingleFlag_ReturnsName()
    {
        var helper = new OperationAccessHelper(OperationAccess.Read);
        Assert.Equal("Read", helper.Describe());
    }

    [Fact]
    public void Describe_MultipleFlags_ReturnsCommaSeparatedNames()
    {
        var helper = new OperationAccessHelper(OperationAccess.Read | OperationAccess.Write);
        var desc = helper.Describe();
        Assert.Contains("Read", desc);
        Assert.Contains("Write", desc);
        Assert.Contains(",", desc);
    }

    [Fact]
    public void HasAny_WithFlags_ReturnsTrue()
    {
        var helper = new OperationAccessHelper(OperationAccess.Read);
        Assert.True(helper.HasAny());
    }

    [Fact]
    public void HasAny_WithNone_ReturnsFalse()
    {
        var helper = new OperationAccessHelper(OperationAccess.None);
        Assert.False(helper.HasAny());
    }

    [Fact]
    public void HasAny_WithOverlappingFlags_ReturnsTrue()
    {
        var helper = new OperationAccessHelper(OperationAccess.Read | OperationAccess.Write);
        Assert.True(helper.HasAny(OperationAccess.Read));
    }

    [Fact]
    public void HasAny_WithNonOverlappingFlags_ReturnsFalse()
    {
        var helper = new OperationAccessHelper(OperationAccess.Read);
        Assert.False(helper.HasAny(OperationAccess.Write));
    }

    [Fact]
    public void HasFlag_WhenFlagPresent_ReturnsTrue()
    {
        var helper = new OperationAccessHelper(OperationAccess.Read | OperationAccess.Write);
        Assert.True(helper.HasFlag(OperationAccess.Read));
    }

    [Fact]
    public void HasFlag_WhenFlagMissing_ReturnsFalse()
    {
        var helper = new OperationAccessHelper(OperationAccess.Read);
        Assert.False(helper.HasFlag(OperationAccess.Write));
    }

    [Fact]
    public void Add_SingleFlag_AddsToValue()
    {
        var helper = new OperationAccessHelper(OperationAccess.None);
        helper.Add(OperationAccess.Read);
        Assert.True(helper.Read);
    }

    [Fact]
    public void Add_MultipleFlags_AccumulatesFlags()
    {
        var helper = new OperationAccessHelper(OperationAccess.Read);
        helper.Add(OperationAccess.Write);
        Assert.True(helper.Read);
        Assert.True(helper.Write);
    }

    [Theory]
    [InlineData("read", OperationAccess.Read)]
    [InlineData("Read", OperationAccess.Read)]
    [InlineData("READ", OperationAccess.Read)]
    [InlineData("write", OperationAccess.Write)]
    [InlineData("create", OperationAccess.Create)]
    [InlineData("delete", OperationAccess.Delete)]
    [InlineData("administrate", OperationAccess.Administrate)]
    [InlineData("configure", OperationAccess.Configure)]
    public void Add_StringArray_ParsesCorrectly(string input, OperationAccess expected)
    {
        var helper = new OperationAccessHelper(OperationAccess.None);
        helper.Add(new[] { input });
        Assert.True(helper.HasFlag(expected));
    }

    [Fact]
    public void Add_MultipleStrings_AddsAllFlags()
    {
        var helper = new OperationAccessHelper(OperationAccess.None);
        helper.Add(new[] { "read", "write", "delete" });

        Assert.True(helper.Read);
        Assert.True(helper.Write);
        Assert.True(helper.Delete);
        Assert.False(helper.Create);
    }

    [Fact]
    public void Add_Wildcard_SetsAllFlags()
    {
        var helper = new OperationAccessHelper(OperationAccess.None);
        helper.Add(new[] { "*" });

        Assert.True(helper.Read);
        Assert.True(helper.Write);
        Assert.True(helper.Create);
        Assert.True(helper.Delete);
        Assert.True(helper.Administrate);
        Assert.True(helper.Configure);
    }

    [Fact]
    public void Add_AllKeyword_SetsAllFlags()
    {
        var helper = new OperationAccessHelper(OperationAccess.None);
        helper.Add(new[] { "ALL" });
        Assert.True(helper.HasAll());
    }

    [Fact]
    public void Add_InvalidString_DoesNotThrow()
    {
        var helper = new OperationAccessHelper(OperationAccess.None);
        helper.Add(new[] { "invalid" });
        Assert.Equal(OperationAccess.None, helper.Value);
    }

    [Fact]
    public void HasAll_WhenAllFlagsSet_ReturnsTrue()
    {
        var all = OperationAccess.Read | OperationAccess.Write | OperationAccess.Create |
                  OperationAccess.Delete | OperationAccess.Administrate | OperationAccess.Configure;
        var helper = new OperationAccessHelper(all);
        Assert.True(helper.HasAll());
    }

    [Fact]
    public void HasAll_WhenSomeFlagsMissing_ReturnsFalse()
    {
        var helper = new OperationAccessHelper(OperationAccess.Read | OperationAccess.Write);
        Assert.False(helper.HasAll());
    }

    [Fact]
    public void HasAll_StringVersion_ChecksSpecificFlag()
    {
        var helper = new OperationAccessHelper(OperationAccess.Read | OperationAccess.Write);
        Assert.True(helper.HasAll("read"));
        Assert.False(helper.HasAll("delete"));
    }
}

public class HttpAccessHelperTests
{
    [Fact]
    public void Constructor_SetsValue()
    {
        var helper = new HttpAccessHelper(HttpAccess.Get);
        Assert.Equal(HttpAccess.Get, helper.Value);
    }

    [Fact]
    public void IsEmpty_WhenNone_ReturnsTrue()
    {
        var helper = new HttpAccessHelper(HttpAccess.None);
        Assert.True(helper.IsEmpty);
    }

    [Theory]
    [InlineData(HttpAccess.Head, true, false, false, false, false)]
    [InlineData(HttpAccess.Get, false, true, false, false, false)]
    [InlineData(HttpAccess.Put, false, false, true, false, false)]
    [InlineData(HttpAccess.Post, false, false, false, true, false)]
    [InlineData(HttpAccess.Delete, false, false, false, false, true)]
    public void IndividualFlags_ReturnCorrectValues(
        HttpAccess access, bool head, bool get, bool put, bool post, bool delete)
    {
        var helper = new HttpAccessHelper(access);
        Assert.Equal(head, helper.Head);
        Assert.Equal(get, helper.Get);
        Assert.Equal(put, helper.Put);
        Assert.Equal(post, helper.Post);
        Assert.Equal(delete, helper.Delete);
    }

    [Fact]
    public void CombinedFlags_WorkCorrectly()
    {
        var access = HttpAccess.Get | HttpAccess.Post | HttpAccess.Delete;
        var helper = new HttpAccessHelper(access);

        Assert.False(helper.Head);
        Assert.True(helper.Get);
        Assert.False(helper.Put);
        Assert.True(helper.Post);
        Assert.True(helper.Delete);
    }

    [Theory]
    [InlineData(HttpAccess.Head, "h")]
    [InlineData(HttpAccess.Get, "g")]
    [InlineData(HttpAccess.Put, "u")]
    [InlineData(HttpAccess.Post, "p")]
    [InlineData(HttpAccess.Delete, "d")]
    public void Abbreviate_SpecificValue_ReturnsCorrectAbbreviation(HttpAccess access, string expected)
    {
        var helper = new HttpAccessHelper(HttpAccess.None);
        Assert.Equal(expected, helper.Abbreviate(access));
    }

    [Fact]
    public void Abbreviate_MultipleFlags_ReturnsCombinedAbbreviations()
    {
        var helper = new HttpAccessHelper(HttpAccess.Get | HttpAccess.Post);
        var abbrev = helper.Abbreviate();
        Assert.Contains("g", abbrev);
        Assert.Contains("p", abbrev);
    }

    [Fact]
    public void Add_Wildcard_SetsAllFlags()
    {
        var helper = new HttpAccessHelper(HttpAccess.None);
        helper.Add(new[] { "*" });

        Assert.True(helper.Head);
        Assert.True(helper.Get);
        Assert.True(helper.Put);
        Assert.True(helper.Post);
        Assert.True(helper.Delete);
    }

    [Theory]
    [InlineData("get", HttpAccess.Get)]
    [InlineData("Get", HttpAccess.Get)]
    [InlineData("GET", HttpAccess.Get)]
    [InlineData("post", HttpAccess.Post)]
    [InlineData("put", HttpAccess.Put)]
    [InlineData("delete", HttpAccess.Delete)]
    [InlineData("head", HttpAccess.Head)]
    public void Add_StringArray_ParsesCorrectly(string input, HttpAccess expected)
    {
        var helper = new HttpAccessHelper(HttpAccess.None);
        helper.Add(new[] { input });
        Assert.True(helper.HasFlag(expected));
    }

    [Fact]
    public void HasAll_WhenAllFlagsSet_ReturnsTrue()
    {
        var all = HttpAccess.Head | HttpAccess.Get | HttpAccess.Put | HttpAccess.Post | HttpAccess.Delete;
        var helper = new HttpAccessHelper(all);
        Assert.True(helper.HasAll());
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
    public void IsEmpty_WhenNone_ReturnsTrue()
    {
        var helper = new AuthorityAccessHelper(AuthorityAccess.None);
        Assert.True(helper.IsEmpty);
    }

    [Fact]
    public void AllIndividualFlags_WorkCorrectly()
    {
        Assert.True(new AuthorityAccessHelper(AuthorityAccess.Visitor).Visitor);
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

        Assert.False(helper.Visitor);
        Assert.False(helper.Member);
        Assert.True(helper.Administrator);
        Assert.True(helper.Developer);
        Assert.True(helper.Operator);
    }

    [Theory]
    [InlineData(AuthorityAccess.Visitor, "x")]
    [InlineData(AuthorityAccess.Member, "e")]
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
        var helper = new AuthorityAccessHelper(AuthorityAccess.None);
        Assert.Equal(expected, helper.Abbreviate(access));
    }

    [Theory]
    [InlineData("visitor", AuthorityAccess.Visitor)]
    [InlineData("Administrator", AuthorityAccess.Administrator)]
    [InlineData("DEVELOPER", AuthorityAccess.Developer)]
    public void Add_StringArray_ParsesCorrectly(string input, AuthorityAccess expected)
    {
        var helper = new AuthorityAccessHelper(AuthorityAccess.None);
        helper.Add(new[] { input });
        Assert.True(helper.HasFlag(expected));
    }

    [Fact]
    public void Add_Wildcard_SetsAllFlags()
    {
        var helper = new AuthorityAccessHelper(AuthorityAccess.None);
        helper.Add(new[] { "*" });

        Assert.True(helper.Visitor);
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
        var all = AuthorityAccess.Visitor | AuthorityAccess.Member | AuthorityAccess.Trainee |
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
    public void Add_FormalSwitchPattern_ParsesCorrectly()
    {
        var access = new Access();
        access.Add("switch:on");
        Assert.Equal(SwitchAccess.On, access.Switch);
    }

    [Fact]
    public void Add_FormalOperationPattern_ParsesCorrectly()
    {
        var access = new Access();
        access.Add("operation:read,write");
        Assert.True(access.Operation.HasFlag(OperationAccess.Read));
        Assert.True(access.Operation.HasFlag(OperationAccess.Write));
    }

    [Fact]
    public void Add_FormalHttpPattern_ParsesCorrectly()
    {
        var access = new Access();
        access.Add("http:get,post");
        Assert.True(access.Http.HasFlag(HttpAccess.Get));
        Assert.True(access.Http.HasFlag(HttpAccess.Post));
    }

    [Fact]
    public void Add_FormalAuthorityPattern_ParsesCorrectly()
    {
        var access = new Access();
        access.Add("authority:administrator,developer");
        Assert.True(access.Authority.HasFlag(AuthorityAccess.Administrator));
        Assert.True(access.Authority.HasFlag(AuthorityAccess.Developer));
    }

    [Fact]
    public void Add_WildcardOperation_SetsAllFlags()
    {
        var access = new Access();
        access.Add("operation:*");
        Assert.True(access.Operation.HasFlag(OperationAccess.Read));
        Assert.True(access.Operation.HasFlag(OperationAccess.Write));
        Assert.True(access.Operation.HasFlag(OperationAccess.Create));
        Assert.True(access.Operation.HasFlag(OperationAccess.Delete));
        Assert.True(access.Operation.HasFlag(OperationAccess.Administrate));
        Assert.True(access.Operation.HasFlag(OperationAccess.Configure));
    }

    [Fact]
    public void Add_AllKeyword_SetsAllFlags()
    {
        var access = new Access();
        access.Add("http:all");
        Assert.True(access.Http.HasFlag(HttpAccess.Head));
        Assert.True(access.Http.HasFlag(HttpAccess.Get));
        Assert.True(access.Http.HasFlag(HttpAccess.Put));
        Assert.True(access.Http.HasFlag(HttpAccess.Post));
        Assert.True(access.Http.HasFlag(HttpAccess.Delete));
    }

    [Theory]
    [InlineData("on")]
    [InlineData("On")]
    [InlineData("ON")]
    public void Add_SimplePattern_SetsSwitchOn(string input)
    {
        var access = new Access();
        access.Add(input);
        Assert.Equal(SwitchAccess.On, access.Switch);
    }

    [Fact]
    public void Add_CaseInsensitive_ParsesCorrectly()
    {
        var access = new Access();
        access.Add("OPERATION:READ,WRITE");
        Assert.True(access.Operation.HasFlag(OperationAccess.Read));
        Assert.True(access.Operation.HasFlag(OperationAccess.Write));
    }

    [Fact]
    public void Add_InvalidPattern_DoesNotThrow()
    {
        var access = new Access();
        access.Add("invalid:pattern:here");
        Assert.Equal(SwitchAccess.Off, access.Switch);
        Assert.Equal(OperationAccess.None, access.Operation);
    }

    [Fact]
    public void Add_AnotherAccessObject_CombinesFlags()
    {
        var access1 = new Access();
        access1.Add("operation:read");

        var access2 = new Access();
        access2.Add("operation:write");
        access2.Add("http:get");

        access1.Add(access2);

        Assert.True(access1.Operation.HasFlag(OperationAccess.Read));
        Assert.True(access1.Operation.HasFlag(OperationAccess.Write));
        Assert.True(access1.Http.HasFlag(HttpAccess.Get));
    }

    [Fact]
    public void HasAny_WhenEmpty_ReturnsFalse()
    {
        var access = new Access();
        Assert.False(access.HasAny());
    }

    [Fact]
    public void HasAny_WhenSwitchSet_ReturnsTrue()
    {
        var access = new Access();
        access.Add("switch:on");
        Assert.True(access.HasAny());
    }

    [Fact]
    public void HasAny_WhenOperationSet_ReturnsTrue()
    {
        var access = new Access();
        access.Add("operation:read");
        Assert.True(access.HasAny());
    }

    [Fact]
    public void HasAny_WhenHttpSet_ReturnsTrue()
    {
        var access = new Access();
        access.Add("http:get");
        Assert.True(access.HasAny());
    }

    [Fact]
    public void HasAny_WhenAuthoritySet_ReturnsTrue()
    {
        var access = new Access();
        access.Add("authority:administrator");
        Assert.True(access.HasAny());
    }

    [Theory]
    [InlineData(SwitchAccess.On, true)]
    [InlineData(SwitchAccess.Off, false)]
    public void Has_SwitchAccess_ReturnsCorrectValue(SwitchAccess check, bool expected)
    {
        var access = new Access();
        access.Add("switch:on");
        Assert.Equal(expected, access.Has(check));
    }

    [Fact]
    public void Has_OperationAccess_ReturnsCorrectValue()
    {
        var access = new Access();
        access.Add("operation:read,write");
        Assert.True(access.Has(OperationAccess.Read));
        Assert.True(access.Has(OperationAccess.Write));
        Assert.False(access.Has(OperationAccess.Delete));
    }

    [Fact]
    public void Has_HttpAccess_ReturnsCorrectValue()
    {
        var access = new Access();
        access.Add("http:get,post");
        Assert.True(access.Has(HttpAccess.Get));
        Assert.True(access.Has(HttpAccess.Post));
        Assert.False(access.Has(HttpAccess.Delete));
    }

    [Fact]
    public void Has_AuthorityAccess_ReturnsCorrectValue()
    {
        var access = new Access();
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
        Assert.Equal(SwitchAccess.On, control.Granted.Switch);
    }

    [Fact]
    public void Add_AllowWithAccess_AddsToGranted()
    {
        var control = new AccessControl();
        control.Add("allow:operation:read");
        Assert.True(control.Granted.Operation.HasFlag(OperationAccess.Read));
    }

    [Fact]
    public void Add_DenyString_AddsToDenied()
    {
        var control = new AccessControl();
        control.Add("deny:switch:on");
        Assert.Equal(SwitchAccess.On, control.Denied.Switch);
    }

    [Fact]
    public void Add_DenyWithAccess_AddsToDenied()
    {
        var control = new AccessControl();
        control.Add("deny:operation:delete");
        Assert.True(control.Denied.Operation.HasFlag(OperationAccess.Delete));
    }

    [Fact]
    public void Add_CaseInsensitive_ParsesCorrectly()
    {
        var control = new AccessControl();
        control.Add("ALLOW:OPERATION:READ");
        Assert.True(control.Granted.Operation.HasFlag(OperationAccess.Read));
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
        control1.Add("allow:operation:read");

        var control2 = new AccessControl();
        control2.Add("allow:operation:write");
        control2.Add("deny:operation:delete");

        control1.Add(control2);

        Assert.True(control1.Granted.Operation.HasFlag(OperationAccess.Read));
        Assert.True(control1.Granted.Operation.HasFlag(OperationAccess.Write));
        Assert.True(control1.Denied.Operation.HasFlag(OperationAccess.Delete));
    }

    [Fact]
    public void Grant_SwitchAccess_AddsToGranted()
    {
        var control = new AccessControl();
        control.Grant(SwitchAccess.On);
        Assert.True(control.IsGranted(SwitchAccess.On));
    }

    [Fact]
    public void Deny_SwitchAccess_AddsToDenied()
    {
        var control = new AccessControl();
        control.Deny(SwitchAccess.On);
        Assert.True(control.IsDenied(SwitchAccess.On));
    }

    [Fact]
    public void Grant_OperationAccess_AddsToGranted()
    {
        var control = new AccessControl();
        control.Grant(OperationAccess.Read);
        Assert.True(control.IsGranted(OperationAccess.Read));
    }

    [Fact]
    public void Deny_OperationAccess_AddsToDenied()
    {
        var control = new AccessControl();
        control.Deny(OperationAccess.Delete);
        Assert.True(control.IsDenied(OperationAccess.Delete));
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
        control.Grant(SwitchAccess.On);
        Assert.True(control.IsAllowed());
    }

    [Fact]
    public void IsAllowed_SwitchAccess_WhenGrantedNotDenied_ReturnsTrue()
    {
        var control = new AccessControl();
        control.Grant(SwitchAccess.On);
        Assert.True(control.IsAllowed(SwitchAccess.On));
    }

    [Fact]
    public void IsAllowed_SwitchAccess_WhenDenied_ReturnsFalse()
    {
        var control = new AccessControl();
        control.Grant(SwitchAccess.On);
        control.Deny(SwitchAccess.On);
        Assert.False(control.IsAllowed(SwitchAccess.On));
    }

    [Fact]
    public void IsAllowed_SwitchAccess_WhenNotGranted_ReturnsFalse()
    {
        var control = new AccessControl();
        Assert.False(control.IsAllowed(SwitchAccess.On));
    }

    [Fact]
    public void IsAllowed_OperationAccess_WhenGrantedNotDenied_ReturnsTrue()
    {
        var control = new AccessControl();
        control.Grant(OperationAccess.Read);
        Assert.True(control.IsAllowed(OperationAccess.Read));
    }

    [Fact]
    public void IsAllowed_OperationAccess_WhenDenied_ReturnsFalse()
    {
        var control = new AccessControl();
        control.Grant(OperationAccess.Read);
        control.Deny(OperationAccess.Read);
        Assert.False(control.IsAllowed(OperationAccess.Read));
    }

    [Fact]
    public void IsAllowed_HttpAccess_WhenGrantedNotDenied_ReturnsTrue()
    {
        var control = new AccessControl();
        control.Granted.Add("http:get");
        Assert.True(control.IsAllowed(HttpAccess.Get));
    }

    [Fact]
    public void IsAllowed_HttpAccess_WhenDenied_ReturnsFalse()
    {
        var control = new AccessControl();
        control.Granted.Add("http:get");
        control.Denied.Add("http:get");
        Assert.False(control.IsAllowed(HttpAccess.Get));
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
    public void IsGranted_SwitchAccess_WhenGranted_ReturnsTrue()
    {
        var control = new AccessControl();
        control.Grant(SwitchAccess.On);
        Assert.True(control.IsGranted(SwitchAccess.On));
    }

    [Fact]
    public void IsGranted_SwitchAccess_WhenNotGranted_ReturnsFalse()
    {
        var control = new AccessControl();
        Assert.False(control.IsGranted(SwitchAccess.On));
    }

    [Fact]
    public void IsDenied_SwitchAccess_WhenDenied_ReturnsTrue()
    {
        var control = new AccessControl();
        control.Deny(SwitchAccess.On);
        Assert.True(control.IsDenied(SwitchAccess.On));
    }

    [Fact]
    public void IsDenied_SwitchAccess_WhenNotDenied_ReturnsFalse()
    {
        var control = new AccessControl();
        Assert.False(control.IsDenied(SwitchAccess.On));
    }

    [Fact]
    public void IsGranted_OperationAccess_WhenGranted_ReturnsTrue()
    {
        var control = new AccessControl();
        control.Grant(OperationAccess.Read);
        Assert.True(control.IsGranted(OperationAccess.Read));
    }

    [Fact]
    public void IsDenied_OperationAccess_WhenDenied_ReturnsTrue()
    {
        var control = new AccessControl();
        control.Deny(OperationAccess.Delete);
        Assert.True(control.IsDenied(OperationAccess.Delete));
    }

    [Fact]
    public void IsGranted_HttpAccess_WhenGranted_ReturnsTrue()
    {
        var control = new AccessControl();
        control.Granted.Add("http:get");
        Assert.True(control.IsGranted(HttpAccess.Get));
    }

    [Fact]
    public void IsDenied_HttpAccess_WhenDenied_ReturnsTrue()
    {
        var control = new AccessControl();
        control.Denied.Add("http:delete");
        Assert.True(control.IsDenied(HttpAccess.Delete));
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
        control.Denied.Add("authority:visitor");
        Assert.True(control.IsDenied(AuthorityAccess.Visitor));
    }

    [Fact]
    public void DenyTakesPrecedence_OverGrant()
    {
        var control = new AccessControl();
        control.Grant(OperationAccess.Read);
        control.Grant(OperationAccess.Write);
        control.Grant(OperationAccess.Delete);
        control.Deny(OperationAccess.Delete);

        Assert.True(control.IsAllowed(OperationAccess.Read));
        Assert.True(control.IsAllowed(OperationAccess.Write));
        Assert.False(control.IsAllowed(OperationAccess.Delete));
    }

    [Fact]
    public void ComplexScenario_MultipleAccessTypes()
    {
        var control = new AccessControl();

        control.Add("allow:switch:on");
        control.Add("allow:operation:read,write,create");
        control.Add("deny:operation:delete");
        control.Add("allow:http:get,post");
        control.Add("allow:authority:administrator,developer");

        Assert.True(control.IsAllowed(SwitchAccess.On));
        Assert.True(control.IsAllowed(OperationAccess.Read));
        Assert.True(control.IsAllowed(OperationAccess.Write));
        Assert.True(control.IsAllowed(OperationAccess.Create));
        Assert.False(control.IsAllowed(OperationAccess.Delete));
        Assert.True(control.IsAllowed(HttpAccess.Get));
        Assert.True(control.IsAllowed(HttpAccess.Post));
        Assert.False(control.IsAllowed(HttpAccess.Delete));
        Assert.True(control.IsAllowed(AuthorityAccess.Administrator));
        Assert.True(control.IsAllowed(AuthorityAccess.Developer));
        Assert.False(control.IsAllowed(AuthorityAccess.Operator));
    }
}

public class EnumFlagsTests
{
    [Fact]
    public void OperationAccess_FlagsArePowersOfTwo()
    {
        Assert.Equal(0, (int)OperationAccess.None);
        Assert.Equal(1, (int)OperationAccess.Read);
        Assert.Equal(2, (int)OperationAccess.Write);
        Assert.Equal(4, (int)OperationAccess.Create);
        Assert.Equal(8, (int)OperationAccess.Delete);
        Assert.Equal(16, (int)OperationAccess.Administrate);
        Assert.Equal(32, (int)OperationAccess.Configure);
    }

    [Fact]
    public void HttpAccess_FlagsArePowersOfTwo()
    {
        Assert.Equal(0, (int)HttpAccess.None);
        Assert.Equal(1, (int)HttpAccess.Head);
        Assert.Equal(2, (int)HttpAccess.Get);
        Assert.Equal(4, (int)HttpAccess.Put);
        Assert.Equal(8, (int)HttpAccess.Post);
        Assert.Equal(16, (int)HttpAccess.Delete);
    }

    [Fact]
    public void AuthorityAccess_FlagsArePowersOfTwo()
    {
        Assert.Equal(0, (int)AuthorityAccess.None);
        Assert.Equal(1, (int)AuthorityAccess.Visitor);
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
    public void SwitchAccess_FlagsAreCorrect()
    {
        Assert.Equal(0, (int)SwitchAccess.Off);
        Assert.Equal(1, (int)SwitchAccess.On);
    }

    [Fact]
    public void CombinedFlags_WorkWithBitwiseOr()
    {
        var combined = OperationAccess.Read | OperationAccess.Write;
        Assert.Equal(3, (int)combined);
    }

    [Fact]
    public void FlagsCanBeCheckedWithHasFlag()
    {
        var combined = OperationAccess.Read | OperationAccess.Write | OperationAccess.Delete;
        Assert.True(combined.HasFlag(OperationAccess.Read));
        Assert.True(combined.HasFlag(OperationAccess.Write));
        Assert.True(combined.HasFlag(OperationAccess.Delete));
        Assert.False(combined.HasFlag(OperationAccess.Create));
    }
}

public class AccessTypeEnumTests
{
    [Theory]
    [InlineData("Switch", AccessType.Switch)]
    [InlineData("Operation", AccessType.Operation)]
    [InlineData("Http", AccessType.Http)]
    [InlineData("Authority", AccessType.Authority)]
    public void AccessType_ParsesCorrectly(string input, AccessType expected)
    {
        var result = Enum.Parse<AccessType>(input, ignoreCase: true);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void AccessType_HasExpectedValues()
    {
        var values = Enum.GetValues<AccessType>();
        Assert.Equal(4, values.Length);
        Assert.Contains(AccessType.Switch, values);
        Assert.Contains(AccessType.Operation, values);
        Assert.Contains(AccessType.Http, values);
        Assert.Contains(AccessType.Authority, values);
    }
}