# Shift iQ

Shift iQ is an open-source learning management system that specializes in the assessment of skills to align talent to career and training pathways. It features a comprehensive, competency-based approach, and it provides a credible record of demonstrated knowledge and achieved skills.

The source code for Shift iQ is most actively developed here in this private repository (`code`) for added security. Source code in this repository is published periodically to the public repository (`shiftiq`), where pull requests from external contributors are welcome. This process ensures the internal commit history is unavailable to the public, which ensures there is no risk of disclosing any personal or confidential information that might exist within the internal development team's commit history.


## Folders

The **build** directory contains the scripts to build release packages.

The **config** directory contains configuration files that are bundled into release packages.

The **source** directory contains the source code files for all .NET Framework 4.8 projects. 

> Note: The Windows file system is case-insensitive, but git repositories are case-sensitive. For example, if a folder is renamed from `TEST` to `test` in the Windows file system, and if git is configured with `core.ignorecase=true`, then this change is not noticed by git, and commands like `git blame` fail unexpectedly. For more details on this issue, refer to https://stackoverflow.com/questions/42077734/git-blame-on-windows-reports-fatal-no-such-path-path-in-head

The **src** directory contains the source code files for all .NET 9.0 and .NET Standard 2.0 projects.


## Files

The **cleanup.ps1** PowerShell script deletes source code subdirectories that are empty (or expected to be empty).

The **solution.sln** Visual Studio solution contains all the projects to build the Shift iQ platform, including .NET Framework 4.8, .NET 9.0, and .NET Standard 2.0 projects. Projects are organized by scope: 

* `kernel` projects contain general-purpose code that is reusable outside the Shift iQ application.

* `application` projects contain code that is specific to the Shift iQ application; this code always executes within the context of a single, specific partition. (Octopus uses the term "tenant" rather than "partition".)

* `infrastructure` projects contain code that implements functionality that is platform-wide. For example, if a maintenance feature operates on all partitions then it is implemented in one of these projects.

The **tasklist.html** page contains a summary of task list comments found in the codebase. This summary report is output by `build/docs/build-tasklist.ps1`.


## Configuration

Configuration settings are stored in the `config` folder. 

Environment-specific settings are optional, and can be defined using these file conventions:

* Application settings: `appsettings.Environment.json` 
* Web configuration settings: `web.Environment.config`

For example, these settings are applied to the Development environment only:

* Application settings: `appsettings.Development.json`
* Web configuration settings: `web.Development.config`

Partition-specific settings are optional also, and can be defined using these file conventions:

* Application settings: `appsettings.Partition.json` 
* Web configuration settings: `web.Partition.config`

For example, these settings are applied to all environments for the E01 (Demo) partition only:

* Application settings: `appsettings.E01.json`
* Web configuration settings: `web.E01.config`

Note: Octopus uses the term **tenant** in the same way that we use the term **partition**. 

Note: The file name extension for an application setting file may be .config or .json - both are supported. However, in both cases, an AppSettings file must store content in JSON format. Only web configuration files store content in XML format.


# Rationale for the MIT license

The MIT license is the best and most suitable license for Shift iQ because educational institutions favor permissive licenses. Schools and universities often have policies against AGPL due to compliance complexity, IT departments prefer licenses with minimal legal overhead, and organizations today are working with much tighter budgets - so it is increasingly important to maximize flexibility and minimize legal expense.

MIT is also a better license for the corporate training market because companies that build internal training systems generally avoid AGPL. In addition, consultants and integrators prefer MIT licensing for custom deployments, for its much greater simplicity and expediency.

In the software development ecosystem, far more developers are willing to contribute to projects that use the MIT license. Plugin and extension developers prefer permissive base platforms, and integration with other tools is easier.

Most successful open-source projects use MIT/Apache licensing for maximum adoption. For example, React, Node.js, Bootstrap, and .NET all use the MIT license, and these are the primary technologies upon which Shift iQ is built. (Moodle and Canvas use GPL and AGPL, respectively, and many organizations avoid them due to their licensing.)

In the Learning Management System domain specifically, organizations want hosting flexibility (i.e., they want the option to choose their hosting without worrying about source code disclosure requirements). Educational institutions want customization freedom (i.e., often they need heavy customization and want to keep some modifications private, such as student data handling and integration with internal/proprietary systems). And, of course, many organizations have procurement rules that make GPL/AGPL compliance too difficult and costly for consideration.

Our primary goal with Shift iQ is adoption, therefore the MIT license removes barriers and encourages the broadest possible use. Much like the education technology space, the Shift iQ development team values open, interoperable solutions - and MIT licensing aligns perfectly with this philosophy.
