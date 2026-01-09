using System;

using Shift.Common;

namespace InSite.Persistence.Plugin.CMDS
{
	[Serializable]
	public class DepartmentUserFilter : Filter
	{
		public Guid UserIdentifier { get; set; }
		public string[] RoleType { get; set; }
		public string SearchText { get; set; }
	}
}
