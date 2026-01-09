using System;

namespace InSite.Persistence
{
    public class RouteNode
    {
        public int RouteDepth { get; set; }
        public string RouteIcon { get; set; }
        public Guid RouteId { get; set; }
        public string RouteList { get; set; }
        public string RouteName { get; set; }
        public string RouteNameShort { get; set; }
        public string RouteType { get; set; }
        public string RouteUrl { get; set; }
        public string RouteUrlIndented { get; set; }
        public string AuthorityType { get; set; }
        public string AuthorizationRequirement { get; set; }
        public string ControllerPath { get; set; }
        public string ExtraBreadcrumb { get; set; }
        public string HelpUrl { get; set; }
        public Guid? ParentRouteId { get; set; }
        public string SortPath { get; set; }
    }
}