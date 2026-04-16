import { useFormWorkshopProvider } from "@/contexts/workshop/FormWorkshopProviderContext";
import { numberHelper } from "@/helpers/numberHelper";

export default function FormWorkshop_FormDetails_Summaries() {
    const { statistics } = useFormWorkshopProvider();

    return (
        <div className="row">
            <div className="col-lg-3">

                <h3>Taxonomies</h3>

                <table className="table table-striped">
                    <thead>
                        <tr>
                            <th>Taxonomy</th>
                            <th className="text-end">Questions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {statistics && statistics.questionPerTaxonomy.map(({ item, count }, index) => (
                            <tr key={index}>
                                <td>{item}</td>
                                <td className="text-end">{numberHelper.formatInt(count)}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
                                
                <h3 className="mt-4">Difficulties</h3>

                <table className="table table-striped">
                    <thead>
                        <tr>
                            <th>Difficulty</th>
                            <th className="text-end">Questions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {statistics && statistics.questionPerDifficulty.map(({ item, count }, index) => (
                            <tr key={index}>
                                <td>{item}</td>
                                <td className="text-end">{numberHelper.formatInt(count)}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>

                <h3 className="mt-4">Like Item Groups</h3>

                <table className="table table-striped mb-0">
                    <thead>
                        <tr>
                            <th>Like Item Group</th>
                            <th className="text-end">Questions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {statistics && statistics.questionPerLIG.map(({ item, count }, index) => (
                            <tr key={index}>
                                <td>{item}</td>
                                <td className="text-end">{numberHelper.formatInt(count)}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>

            </div>
            <div className="col-lg-3">

                <h3 className="mt-4 mt-lg-0">Standards (GAC)</h3>

                <table className="table table-striped mb-0">
                    <thead>
                        <tr>
                            <th>Standard</th>
                            <th className="text-end">Questions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {statistics && statistics.questionPerGAC.map(({ item, count }, index) => (
                            <tr key={index}>
                                <td>{item}</td>
                                <td className="text-end">{numberHelper.formatInt(count)}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>

                <h3 className="mt-4 mt-lg-0">Codes</h3>

                <table className="table table-striped mb-0">
                    <thead>
                        <tr>
                            <th>Code</th>
                            <th className="text-end">Questions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {statistics && statistics.questionPerCode.map(({ item, count }, index) => (
                            <tr key={index}>
                                <td>{item}</td>
                                <td className="text-end">{numberHelper.formatInt(count)}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>

            </div>
            <div className="col-lg-6">

                <h3 className="mt-4 mt-lg-0">Standards (Competency)</h3>

                <table className="table table-striped">
                    <thead>
                        <tr>
                            <th>Standard</th>
                            <th className="text-end">Questions</th>
                            {statistics && statistics.taxonomies.map((t, index) => (
                                <th key={index} className="text-end">Tax {t}</th>
                            ))}
                        </tr>
                    </thead>
                    <tbody>
                        {statistics && statistics.standards.map((s, index) => (
                            <tr key={index}>
                                <td>
                                    {s.setStandardCode ? s.setStandardCode : <strong>?</strong>}
                                    {s.questionStandardCode ? s.questionStandardCode : <strong>?</strong>}
                                </td>
                                <td className="text-end">
                                    {numberHelper.formatInt(s.questions)}
                                </td>
                                {s.taxonomies.map((t, index) => (
                                    <td key={index} className="text-end">{t ? numberHelper.formatInt(t) : null}</td>
                                ))}
                            </tr>
                        ))}
                    </tbody>
                </table>

                <h3 className="mt-4">Standards (Sub Competency)</h3>

                <table className="table table-striped">
                    <thead>
                        <tr>
                            <th>Standard</th>
                            <th className="text-end">Questions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {statistics && statistics.subCompetencies.map((s, index) => (
                            <tr key={index}>
                                <td>
                                    {s.setStandardCode ? s.setStandardCode : <strong>?</strong>}
                                    {s.questionStandardCode ? s.questionStandardCode : <strong>?</strong>}
                                    {s.questionSubCode ? s.questionSubCode : <strong>?</strong>}
                                </td>
                                <td className="text-end">
                                    {numberHelper.formatInt(s.questions)}
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>

                <h3 className="mt-4">Tags</h3>

                <table className="table table-striped">
                    <thead>
                        <tr>
                            <th>Tag</th>
                            <th className="text-end">Questions</th>
                            {statistics && statistics.questionPerTaxonomy.map(({ item }, index) => (
                                <th key={index} className="text-end">Tax {item}</th>
                            ))}
                        </tr>
                    </thead>
                    <tbody>
                        {statistics && statistics.tagAndTaxonomy.map((t, index) => (
                            <tr key={index}>
                                <td>{t.tag}</td>
                                <td className="text-end">
                                    {t.questions}
                                </td>
                                {t.countPerTaxonomy.map((c, index) => (
                                    <td key={index} className="text-end">{numberHelper.formatInt(c)}</td>
                                ))}
                            </tr>
                        ))}
                    </tbody>
                </table>

            </div>
        </div>
    );
}