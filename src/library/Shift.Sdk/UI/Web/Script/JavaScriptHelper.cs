using System.Collections.Generic;

using Shift.Common;

namespace Shift.Sdk.UI
{
    public static class JavaScriptHelper
    {
        public static class GridReorder
        {
            public abstract class ReorderInfo
            {
                public int ContainerIndex { get; protected set; }
                public int ItemIndex { get; protected set; }

                public object Container { get; set; }
            }

            public class DestinationInfo : ReorderInfo
            {
                public DestinationInfo(int container, int item)
                {
                    ContainerIndex = container;
                    ItemIndex = item;
                }
            }

            public class SourceInfo : ReorderInfo
            {
                public object Item { get; set; }

                public SourceInfo(int container, int item)
                {
                    ContainerIndex = container;
                    ItemIndex = item;
                }
            }

            public class ReorderMove
            {
                public SourceInfo Source { get; private set; }
                public DestinationInfo Destination { get; private set; }

                public ReorderMove(SourceInfo source, DestinationInfo destination)
                {
                    Source = source;
                    Destination = destination;
                }
            }

            public static IEnumerable<ReorderMove> Parse(string value)
            {
                var result = new List<ReorderMove>();

                if (!string.IsNullOrEmpty(value))
                {
                    var containers = value.Split(';');
                    for (int destinationContainerIndex = 0; destinationContainerIndex < containers.Length; destinationContainerIndex++)
                    {
                        var containerInfo = containers[destinationContainerIndex];
                        if (string.IsNullOrEmpty(containerInfo))
                            continue;

                        var items = containerInfo.Split(',');
                        for (int destinationItemIndex = 0; destinationItemIndex < items.Length; destinationItemIndex++)
                        {
                            var isError = true;

                            var itemInfo = items[destinationItemIndex];
                            if (!string.IsNullOrEmpty(itemInfo))
                            {
                                var sourceValues = itemInfo.Split(':');
                                if (sourceValues.Length == 2)
                                {
                                    var sourceContainerIndex = int.Parse(sourceValues[0]);
                                    var sourceItemIndex = int.Parse(sourceValues[1]);

                                    if (destinationContainerIndex != sourceContainerIndex || destinationItemIndex != sourceItemIndex)
                                    {
                                        result.Add(
                                            new ReorderMove(
                                                new SourceInfo(sourceContainerIndex, sourceItemIndex),
                                                new DestinationInfo(destinationContainerIndex, destinationItemIndex)
                                            )
                                        );
                                    }

                                    isError = false;
                                }
                            }

                            if (isError)
                                throw ApplicationError.Create("Invalid request: {0}", value);
                        }
                    }
                }

                return result.ToArray();
            }
        }
    }
}