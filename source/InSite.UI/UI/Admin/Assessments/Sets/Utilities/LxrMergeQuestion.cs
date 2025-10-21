using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Shift.Common;
using Shift.Common.File;

namespace InSite.Admin.Assessments.Sets.Utilities
{
    public class LxrMergeQuestion
    {
        #region Properties

        public string Sequence { get; private set; }
        public string Code { get; private set; }
        public string Text { get; private set; }

        public string Competency { get; private set; }
        public string Taxonomy { get; private set; }
        public string LikeItemGroup { get; private set; }
        public string Tag { get; private set; }
        public string Reference { get; private set; }

        public string Difficulty { get; private set; }
        public string IncorrectRationale { get; private set; }
        public string CorrectRationale { get; private set; }

        public IReadOnlyList<QuestionOption> Options => _options;

        #endregion

        #region Fields

        private List<QuestionOption> _options = new List<QuestionOption>();

        #endregion

        #region Construction

        private LxrMergeQuestion()
        {

        }

        #endregion

        #region Methods (parse file)

        public static LxrMergeQuestion[] Read(Stream stream, Encoding encoding)
        {
            var list = new List<LxrMergeQuestion>();
            var table = LxrMergeFile.ToDataTable(stream, new LxrMergeFile.Options
            {
                IncludeColumns = new[] { "OBJ", "SEQ", "STEM", "ANSWER", "CHOICE_A", "CHOICE_B", "CHOICE_C", "CHOICE_D", "CAT_1", "CAT_2", "CAT_3", "CAT_4", "NOTES", "RATIONALE_1", "RATIONALE_2", "DIFFICULTY" },
                RequiredColumns = new[] { "OBJ", "SEQ", "STEM", "ANSWER", "CHOICE_A", "CHOICE_B", "CHOICE_C", "CHOICE_D", "CAT_1", "CAT_2", "CAT_3", "CAT_4", "NOTES" },
                Encoding = encoding
            });

            var hasRationale1 = table.Columns.Contains("RATIONALE_1");
            var hasRationale2 = table.Columns.Contains("RATIONALE_2");
            var hasDifficulty = table.Columns.Contains("DIFFICULTY");

            for (var i = 0; i < table.Rows.Count; i++)
            {
                var row = new DataRowHelper(table.Rows[i]);

                var q = new LxrMergeQuestion
                {
                    Code = row.GetString("OBJ"),
                    Sequence = row.GetString("SEQ"),
                    Text = row.GetString("STEM"),
                };


                if (string.IsNullOrEmpty(q.Sequence))
                    throw new ApplicationError($"Missing question sequence on row {i}");

                q.Code += " " + q.Sequence.PadLeft(3, '0');

                if (string.IsNullOrEmpty(q.Text))
                    throw new ApplicationError($"Missing question stem on row {i}: {q.Code} {q.Sequence}");

                var answer = row.GetString("ANSWER");

                q._options.Add(new QuestionOption(row.GetString("CHOICE_A"), string.Equals(answer, "A", StringComparison.OrdinalIgnoreCase)));
                q._options.Add(new QuestionOption(row.GetString("CHOICE_B"), string.Equals(answer, "B", StringComparison.OrdinalIgnoreCase)));
                q._options.Add(new QuestionOption(row.GetString("CHOICE_C"), string.Equals(answer, "C", StringComparison.OrdinalIgnoreCase)));
                q._options.Add(new QuestionOption(row.GetString("CHOICE_D"), string.Equals(answer, "D", StringComparison.OrdinalIgnoreCase)));

                var cat1 = row.GetString("CAT_1");
                var cat2 = row.GetString("CAT_2");
                var cat3 = row.GetString("CAT_3");
                var cat4 = row.GetString("CAT_4");
                var notes = row.GetString("NOTES");
                var rationale1 = hasRationale1 ? row.GetString("RATIONALE_1") : null;
                var rationale2 = hasRationale2 ? row.GetString("RATIONALE_2") : null;
                var difficulty = hasDifficulty ? row.GetString("DIFFICULTY") : null;

                q.Competency = cat1;
                q.Taxonomy = cat2;
                q.LikeItemGroup = cat3;
                q.Tag = cat4;
                q.Reference = notes;
                q.CorrectRationale = rationale1;
                q.IncorrectRationale = rationale2;
                q.Difficulty = difficulty;

                if (q.Reference != null && q.Reference.Length > 256)
                    q.Reference = q.Reference.Substring(0, 256);

                list.Add(q);
            }

            return list.ToArray();
        }

        #endregion
    }
}