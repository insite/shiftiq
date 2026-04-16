import { textHelper } from "@/helpers/textHelper";
import { ReactNode } from "react";

interface Props {
    children?: ReactNode | null;
}

export default function TextOrNone({ children }: Props) {
    return children ? children : textHelper.none();
}