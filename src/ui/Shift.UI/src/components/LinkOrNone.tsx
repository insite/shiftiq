import { textHelper } from "@/helpers/textHelper";
import ActionLink from "./ActionLink";

interface Props {
    href?: string | null;
    text?: string | null;
}

export default function LinkOrNone({
    href,
    text
}: Props) {
    if (!href || !text) {
        return textHelper.none();
    }
    return (
        <ActionLink href={href}>
            {text}
        </ActionLink>
    )
}