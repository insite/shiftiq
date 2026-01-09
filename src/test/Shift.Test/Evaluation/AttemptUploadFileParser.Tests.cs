using Shift.Common.File;

namespace Shift.Test.Evaluation
{
    public class AttemptUploadFileParserTests
    {
        [Fact]
        public void Parse_CsvContainingMultipleForms_Success()
        {
            var csv = @"ITA Individual ID,Name,Q1,Q2,Q3
900001,Miller Daniel,A,B
900003,Coetzee-Khan Stefan,A,A
900002,Erickson Kyle,C,B,A";
            var csvType = AttemptUploadFileParser.CsvType_CodeNameQuestions.ID;

            var stream = GenerateStreamFromString(csv);
            var lines = AttemptUploadFileParser.ParseCsv(stream, csvType, System.Text.Encoding.UTF8);

            // There are 3 lines of assessment attempt answers.
            Assert.Equal(3, lines.Length);

            // Stefan and Daniel answered a form with 2 questions.
            Assert.Equal(2, lines[0].AttemptAnswers.Length);
            Assert.Equal(2, lines[1].AttemptAnswers.Length);

            // Kyle answered a form with 3 questions.
            Assert.Equal(3, lines[2].AttemptAnswers.Length);
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}