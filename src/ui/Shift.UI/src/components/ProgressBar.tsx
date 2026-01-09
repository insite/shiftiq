export interface ProgressBarProps {
    minValue?: number;
    maxValue?: number;
    value?: number;
    height?: string | number;
    showLabel?: boolean;
    labelAsPercentage?: boolean;
    className?: string;
    indicator?: ProgressBarIndicator;
    indicatorAsBg?: boolean;
    striped?: boolean;
    animated?: boolean;
}

export type ProgressBarIndicator = "default" | "primary" | "secondary" | "success" | "danger" | "warning" | "info" | "light" | "dark";

// eslint-disable-next-line react-refresh/only-export-components
export const ProgressBarIndicator: Record<ProgressBarIndicator, ProgressBarIndicator> = {
    default: "default",
    primary: "primary",
    secondary: "secondary",
    success: "success",
    danger: "danger",
    warning: "warning",
    info: "info",
    light: "light",
    dark: "dark"
};

function getProgressBarClass(indicator: ProgressBarIndicator, indicatorAsBg: boolean, striped: boolean, animated: boolean): { progressClass: string; progressBarClass: string } {
    let progressClass = "progress";
    let progressBarClass = "progress-bar";

    if (indicator !== ProgressBarIndicator.default) {
        if (indicatorAsBg) {
            progressClass += ` bg-${indicator}-subtle`;
        }

        progressBarClass += ` bg-${indicator}`;
    }

    if (striped) {
        progressBarClass += " progress-bar-striped";
        if (animated) {
            progressBarClass += " progress-bar-animated";
        }
    }

    return { progressClass, progressBarClass };
}

export default function ProgressBar({
    minValue = 0,
    maxValue = 100,
    value = 0,
    height,
    showLabel = true,
    labelAsPercentage = true,
    className,
    indicator = ProgressBarIndicator.default,
    indicatorAsBg = false,
    striped = false,
    animated = false
}: ProgressBarProps) {
    if (minValue >= maxValue) {
        throw new Error(`ProgressBar minValue must be less than maxValue. Received: minValue=${minValue}, maxValue=${maxValue}`);
    }

    if (value < minValue) {
        value = minValue;
    } else if (value > maxValue) {
        value = maxValue;
    }

    const { progressClass, progressBarClass } = getProgressBarClass(indicator, indicatorAsBg, striped, animated);
    const progress = Math.round(((value - minValue) / (maxValue - minValue)) * 100);

    return (
        <div className={progressClass + (className ? ` ${className}` : "")} style={{ height: height }}>
            <div
                className={progressBarClass}
                role="progressbar"
                aria-valuenow={value}
                aria-valuemin={minValue}
                aria-valuemax={maxValue}
                style={{ width: `${progress}%` }}
            >
                {showLabel && (
                    labelAsPercentage ? `${progress}%` : value
                )}
            </div>
        </div>
    );
}