using System.ComponentModel;

using Shift.Service.Chatbot.Tools;

namespace Shift.Service.Chatbot.ShiftTools.Common;

internal record DisplayCsvToolArgs(
    [property: Description("The CSV FileId returned by export function.")]
    Guid FileId,

    [property: Description("The list of columns to display in the table. The order of columns defines how they will be displayed in the table.")]
    string[] Columns,

    [property: Description("Maximum number of rows to display. The hard limit is 100.")]
    int? Limit = null
);

internal class DisplayCsvTool(CsvStore store) :
    BaseTool<DisplayCsvToolArgs, string>(
        "display_csv_as_table",
        """
        Displays the content of a previously exported CSV file (by FileId) as a table.
        It will be displayed directly in the chat after the tool is called, you will only receive either `Displayed` or `No data found`.
        Therefore, DO NOT display the table by yourself.
        Is limited by 100 first rows.
        """,
        null
    )
{
    protected override async Task<InternalToolResponse> InternalExecuteAsync(ToolContext context, DisplayCsvToolArgs? args)
    {
        ArgumentNullException.ThrowIfNull(args);

        var limit = Math.Clamp(args.Limit ?? 100, 0, 100);

        var table = await store.ConvertToTableAsync(context.Principal, args.FileId, args.Columns, limit);

        return table != null
            ? new InternalToolResponse("Displayed", null, table)
            : new InternalToolResponse("No data found");
    }
}