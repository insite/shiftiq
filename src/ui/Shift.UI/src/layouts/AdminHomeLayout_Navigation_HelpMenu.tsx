import Icon from "@/components/icon/Icon";
import { useSiteProvider } from "@/contexts/site/SiteProviderContext";

export default function AdminHomeLayout_Navigation_HelpMenu() {
    const { siteSetting } = useSiteProvider();

    return (
        <li className="nav-item dropdown fs-sm">

            <a href="#" className="nav-link dropdown-toggle" data-bs-toggle="dropdown" data-bs-auto-close="outside" aria-expanded="false">
                <Icon style="regular" name="question-circle" className="me-2 fa-width-auto" />
                Help
            </a>

            <ul className="dropdown-menu dropdown-menu-end">
                
                {siteSetting.UserName && siteSetting.IsCmds && (
                    <>
                        <li>
                            <h6 className="dropdown-header pb-1">Get Help</h6>
                        </li>
                        <li><a className="dropdown-item ms-2" href="/ui/portal/support">Submit a support request</a></li>
                        <li><a className="dropdown-item ms-2" target="_blank" href="https://www.keyeracmds.com/blog">Blog posts, news, and updates</a></li>
                        <li><a className="dropdown-item ms-2" target="_blank" href="https://hub.cmds.app/lobby/docs/guides/terminology.pdf">Terminology</a></li>
                        <li>
                            <h6 className="dropdown-header pb-1">CMDS Guides</h6>
                        </li>
                        <li><a className="dropdown-item ms-2" target="_blank" href="https://hub.cmds.app/lobby/docs/guides/learner.pdf">User guide</a></li>
                        <li><a className="dropdown-item ms-2" target="_blank" href="https://hub.cmds.app/lobby/docs/guides/validator.pdf">Validator guide</a></li>
                        <li><a className="dropdown-item ms-2" target="_blank" href="https://hub.cmds.app/lobby/docs/guides/administrator.pdf">Administrator guide</a></li>
                        <li>
                            <h6 className="dropdown-header pb-1">Orientation Guides (Skills Passport)</h6>
                        </li>
                        <li><a className="dropdown-item ms-2" href="https://hub.cmds.app/lobby/docs/guides/orientation.pdf">Orientations and certificates</a></li>
                        <li>
                            <h6 className="dropdown-header pb-1">Course Registration Guides</h6>
                        </li>
                        <li><a className="dropdown-item ms-2" href="https://hub.cmds.app/lobby/docs/guides/learning-catalogue-and-registration.pdf">Keyera's learning catalogue &amp; registration guide</a></li>
                    </>
                )}

                <li><h6 className="dropdown-header pt-2 pb-1">Resources</h6></li>
                <li><a className="dropdown-item ms-2 disabled" href="https://docs.shiftiq.com/help">Help center</a></li>
                <li><a className="dropdown-item ms-2 disabled" href="https://docs.shiftiq.com/">Documentation</a></li>

                {siteSetting.UserName && !siteSetting.IsCmds && (
                    <>
                        <li><h6 className="dropdown-header pb-1">Get Help</h6></li>
                        <li><a className="dropdown-item ms-2" href="/ui/portal/support">Contact Support</a></li>
                    </>
                )}
                
            </ul>

        </li>
    );
}