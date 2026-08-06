import "./CapabilityReport.css";

type CapabilitySector = {
    innerLabel: string;
    outerLabel: string;
    partialLabel: string;
    standards: string[];
};

type CapabilityStandardExtension = {
    sectorIndex: number;
    labels: string[];
    bandWidth: number;
};

type RingBand = {
    innerRadius: number;
    outerRadius: number;
};

type Point = {
    x: number;
    y: number;
};

type CapabilityLegendColorKey = "standards" | "satisfied" | "partiallySatisfied";
type CapabilityColorKey = CapabilityLegendColorKey | "outline" | "text" | "legendText";

type CapabilityFramework = {
    colors: Record<CapabilityColorKey, string>;
    legend: { label: string; colorKey: CapabilityLegendColorKey }[];
    sectors: CapabilitySector[];
    standardExtensions: CapabilityStandardExtension[];
};

const capabilityFramework: CapabilityFramework = {
    colors: {
        standards: "#0067b9",
        satisfied: "#009f4d",
        partiallySatisfied: "#f7b51f",
        outline: "#05233a",
        text: "#ffffff",
        legendText: "#9a8d9b",
    },
    legend: [
        { label: "AHRCF Standards", colorKey: "standards" },
        { label: "Satisfied", colorKey: "satisfied" },
        { label: "Partially Satisfied", colorKey: "partiallySatisfied" },
    ],
    sectors: [
        {
            innerLabel: "Health Safety\n& Wellbeing",
            outerLabel: "Workplace\nHealth & Safety",
            partialLabel: "Wellbeing",
            standards: ["Psychological Health", "Psychological Safety"],
        },
        {
            innerLabel: "Culture &\nDiversity",
            outerLabel: "Culture",
            partialLabel: "Ethical Practice",
            standards: ["Diversity, Equity & Inclusion"],
        },
        {
            innerLabel: "Business\nEffectiveness",
            outerLabel: "Workforce Planning",
            partialLabel: "Industrial Relations",
            standards: [],
        },
        {
            innerLabel: "Talent\nManagement",
            outerLabel: "Talent\nAcquisition",
            partialLabel: "Capability",
            standards: [],
        },
        {
            innerLabel: "Trusted\nPartnerships",
            outerLabel: "Influence & Impact",
            partialLabel: "Coaching & Empowerment",
            standards: ["Employee Relations"],
        },
        {
            innerLabel: "Business\nStrategy",
            outerLabel: "Strategic\nLeadership",
            partialLabel: "HR Strategy",
            standards: ["Business Acumen"],
        },
        {
            innerLabel: "Organisational\nEnablement",
            outerLabel: "Change\nManagement",
            partialLabel: "HR Operations\n& Compliance",
            standards: [],
        },
    ],
    standardExtensions: [
        {
            sectorIndex: 6,
            labels: ["Data, Analytics & Insights", "Artificial Intelligence", "Technology"],
            bandWidth: 90,
        },
        {
            sectorIndex: 3,
            labels: ["Performance Management", "Reward & Recognition", "Succession Planning", "Engagement & Retention"],
            bandWidth: 95,
        },
        {
            sectorIndex: 2,
            labels: ["Organisational Design", "Work Design"],
            bandWidth: 90,
        },
    ],
};

const center = { x: 595, y: 540 };
const viewBox = { x: 0, y: -180, width: 1260, height: 1580 };
const segmentAngle = 360 / capabilityFramework.sectors.length;
const startAngle = -90;
const wheelLevels = {
    centerRadius: 116,
    innerSatisfied: { innerRadius: 116, outerRadius: 220 },
    outerSatisfied: { innerRadius: 220, outerRadius: 330 },
    partiallySatisfied: { innerRadius: 330, outerRadius: 430 },
    standards: { innerRadius: 430, outerRadius: 560 },
};

export default function CapabilityReport() {
    return (
        <div className="CapabilityReport">
            <div className="capability-report-canvas">
                <svg
                    className="capability-wheel"
                    viewBox={`${viewBox.x} ${viewBox.y} ${viewBox.width} ${viewBox.height}`}
                    role="img"
                    aria-labelledby="capability-wheel-title capability-wheel-description"
                >
                    <title id="capability-wheel-title">Capability framework wheel</title>
                    <desc id="capability-wheel-description">
                        AHRI capability framework wheel showing satisfied, partially satisfied, and AHRCF standards capability areas.
                    </desc>

                    <g className="capability-wheel-frame">
                        {capabilityFramework.sectors.map((sector, index) => (
                            <CapabilitySectorShapes key={sector.innerLabel} sector={sector} index={index} />
                        ))}
                        <CapabilityStandardExtensionShapes />
                        <polygon
                            className="capability-wheel-segment capability-wheel-segment-satisfied"
                            fill={capabilityFramework.colors.satisfied}
                            stroke={capabilityFramework.colors.outline}
                            points={getPolygonPoints(wheelLevels.centerRadius)}
                        />
                        <AhriLogo />
                        {capabilityFramework.sectors.map((sector, index) => (
                            <CapabilitySectorLabels key={sector.outerLabel} sector={sector} index={index} />
                        ))}
                        <CapabilityStandardExtensionLabels />
                    </g>

                    <CapabilityLegend />
                </svg>
            </div>
        </div>
    );
}

function CapabilitySectorShapes({ sector, index }: {
    sector: CapabilitySector;
    index: number;
}) {
    const { start, end } = getSectorAngles(index);
    const { colors } = capabilityFramework;

    return (
        <g>
            {sector.standards.map((standard, standardIndex) => (
                <path
                    key={standard}
                    className="capability-wheel-segment capability-wheel-segment-standards"
                    fill={colors.standards}
                    stroke={colors.outline}
                    d={getSectorBandPath(start, end, getStandardBand(standardIndex, sector.standards.length))}
                />
            ))}
            <path
                className="capability-wheel-segment capability-wheel-segment-partial"
                fill={colors.partiallySatisfied}
                stroke={colors.outline}
                d={getSectorBandPath(start, end, wheelLevels.partiallySatisfied)}
            />
            <path
                className="capability-wheel-segment capability-wheel-segment-satisfied"
                fill={colors.satisfied}
                stroke={colors.outline}
                d={getSectorBandPath(start, end, wheelLevels.outerSatisfied)}
            />
            <path
                className="capability-wheel-segment capability-wheel-segment-satisfied"
                fill={colors.satisfied}
                stroke={colors.outline}
                d={getSectorBandPath(start, end, wheelLevels.innerSatisfied)}
            />
        </g>
    );
}

function CapabilityStandardExtensionShapes() {
    const { colors } = capabilityFramework;

    return (
        <g>
            {capabilityFramework.standardExtensions.flatMap(extension => {
                const { start, end } = getSectorAngles(extension.sectorIndex);

                return extension.labels.map((label, labelIndex) => (
                    <path
                        key={label}
                        className="capability-wheel-segment capability-wheel-segment-standards"
                        fill={colors.standards}
                        stroke={colors.outline}
                        d={getSectorBandPath(start, end, getExtensionBand(extension, labelIndex))}
                    />
                ));
            })}
        </g>
    );
}

function CapabilitySectorLabels({ sector, index }: {
    sector: CapabilitySector;
    index: number;
}) {
    const { start, end, middle } = getSectorAngles(index);
    const rotation = getReadableRotation(middle + 90);

    return (
        <g>
            {sector.standards.map((standard, standardIndex) => (
                <SvgLabel
                    key={standard}
                    label={standard}
                    point={getLabelPoint(middle, getBandMidpointRadius(getStandardBand(standardIndex, sector.standards.length)))}
                    rotation={rotation}
                    className="capability-wheel-standard-label"
                />
            ))}
            <SvgLabel
                label={sector.partialLabel}
                point={getLabelPoint(middle, getBandMidpointRadius(wheelLevels.partiallySatisfied))}
                rotation={rotation}
                className="capability-wheel-partial-label"
            />
            <SvgLabel
                label={sector.outerLabel}
                point={getLabelPoint(middle, getBandMidpointRadius(wheelLevels.outerSatisfied))}
                rotation={rotation}
                className="capability-wheel-satisfied-label"
            />
            <SvgLabel
                label={sector.innerLabel}
                point={getLabelPoint(middle, getBandMidpointRadius(wheelLevels.innerSatisfied))}
                rotation={rotation}
                className="capability-wheel-core-label"
            />
            <line
                className="capability-wheel-spoke"
                stroke={capabilityFramework.colors.outline}
                x1={polarToCartesian(wheelLevels.centerRadius, start).x}
                y1={polarToCartesian(wheelLevels.centerRadius, start).y}
                x2={polarToCartesian(getSectorMaxRadius(index), start).x}
                y2={polarToCartesian(getSectorMaxRadius(index), start).y}
            />
            {index === capabilityFramework.sectors.length - 1 && (
                <line
                    className="capability-wheel-spoke"
                    stroke={capabilityFramework.colors.outline}
                    x1={polarToCartesian(wheelLevels.centerRadius, end).x}
                    y1={polarToCartesian(wheelLevels.centerRadius, end).y}
                    x2={polarToCartesian(getSectorMaxRadius(index), end).x}
                    y2={polarToCartesian(getSectorMaxRadius(index), end).y}
                />
            )}
        </g>
    );
}

function CapabilityStandardExtensionLabels() {
    return (
        <g>
            {capabilityFramework.standardExtensions.flatMap(extension => {
                const { middle } = getSectorAngles(extension.sectorIndex);
                const rotation = getReadableRotation(middle + 90);

                return extension.labels.map((label, labelIndex) => (
                    <SvgLabel
                        key={label}
                        label={label}
                        point={getLabelPoint(middle, getBandMidpointRadius(getExtensionBand(extension, labelIndex)))}
                        rotation={rotation}
                        className="capability-wheel-standard-label"
                    />
                ));
            })}
        </g>
    );
}

function SvgLabel({ label, point, rotation, className }: {
    label: string;
    point: Point;
    rotation: number;
    className: string;
}) {
    const lines = label.split("\n");
    const lineHeight = 25;
    const firstLineOffset = -((lines.length - 1) * lineHeight) / 2;

    return (
        <text
            className={`capability-wheel-label ${className}`}
            fill={capabilityFramework.colors.text}
            x={point.x}
            y={point.y}
            textAnchor="middle"
            dominantBaseline="middle"
            transform={`rotate(${rotation} ${point.x} ${point.y})`}
        >
            {lines.map((line, index) => (
                <tspan key={line} x={point.x} dy={index === 0 ? firstLineOffset : lineHeight}>
                    {line}
                </tspan>
            ))}
        </text>
    );
}

function AhriLogo() {
    return (
        <g className="capability-ahri-logo" fill={capabilityFramework.colors.text} transform={`translate(${center.x} ${center.y})`}>
            <text className="capability-ahri-logo-main" x="-78" y="-6">AHRI</text>
            <path className="capability-ahri-logo-corner" stroke={capabilityFramework.colors.text} d="M 94 -67 h 38 v 38 M 102 -59 h 22 v 22" />
            <text className="capability-ahri-logo-subtitle" x="-76" y="34">Australian</text>
            <text className="capability-ahri-logo-subtitle" x="-76" y="66">HR Institute.</text>
        </g>
    );
}

function CapabilityLegend() {
    const legendX = 900;
    const legendY = 1130;

    return (
        <g className="capability-wheel-legend">
            {capabilityFramework.legend.map((item, index) => (
                <g key={item.label} transform={`translate(${legendX} ${legendY + index * 72})`}>
                    <circle r="22" fill={capabilityFramework.colors[item.colorKey]} />
                    <text x="52" y="10" fill={capabilityFramework.colors.legendText}>{item.label}</text>
                </g>
            ))}
        </g>
    );
}

function getSectorAngles(index: number) {
    const start = startAngle + index * segmentAngle;
    const end = start + segmentAngle;

    return {
        start,
        end,
        middle: (start + end) / 2,
    };
}

function getSectorBandPath(start: number, end: number, band: RingBand) {
    return getRingSegmentPath(start, end, band.innerRadius, band.outerRadius);
}

function getStandardBand(index: number, labelCount: number): RingBand {
    const bandWidth = (wheelLevels.standards.outerRadius - wheelLevels.standards.innerRadius) / labelCount;

    return {
        innerRadius: wheelLevels.standards.innerRadius + bandWidth * index,
        outerRadius: wheelLevels.standards.innerRadius + bandWidth * (index + 1),
    };
}

function getExtensionBand(extension: CapabilityStandardExtension, index: number): RingBand {
    return {
        innerRadius: wheelLevels.standards.innerRadius + extension.bandWidth * index,
        outerRadius: wheelLevels.standards.innerRadius + extension.bandWidth * (index + 1),
    };
}

function getBandMidpointRadius(band: RingBand) {
    return (band.innerRadius + band.outerRadius) / 2;
}

function getSectorMaxRadius(sectorIndex: number) {
    const extension = capabilityFramework.standardExtensions.find(x => x.sectorIndex === sectorIndex);

    if (!extension) {
        return wheelLevels.standards.outerRadius;
    }

    return getExtensionBand(extension, extension.labels.length - 1).outerRadius;
}

function getRingSegmentPath(start: number, end: number, innerRadius: number, outerRadius: number) {
    const outerStart = polarToCartesian(outerRadius, start);
    const outerEnd = polarToCartesian(outerRadius, end);
    const innerEnd = polarToCartesian(innerRadius, end);
    const innerStart = polarToCartesian(innerRadius, start);

    return [
        `M ${formatPoint(outerStart)}`,
        `L ${formatPoint(outerEnd)}`,
        `L ${formatPoint(innerEnd)}`,
        `L ${formatPoint(innerStart)}`,
        "Z",
    ].join(" ");
}

function getPolygonPoints(radius: number) {
    return capabilityFramework.sectors
        .map((_, index) => polarToCartesian(radius, startAngle + index * segmentAngle))
        .map(formatPoint)
        .join(" ");
}

function getLabelPoint(angle: number, radius: number) {
    return polarToCartesian(radius, angle);
}

function polarToCartesian(radius: number, angle: number): Point {
    const angleInRadians = (angle * Math.PI) / 180;

    return {
        x: center.x + radius * Math.cos(angleInRadians),
        y: center.y + radius * Math.sin(angleInRadians),
    };
}

function formatPoint(point: Point) {
    return `${formatNumber(point.x)},${formatNumber(point.y)}`;
}

function formatNumber(value: number) {
    return Number(value.toFixed(3));
}

function getReadableRotation(rotation: number) {
    let normalized = ((rotation + 180) % 360) - 180;

    if (normalized > 90) {
        normalized -= 180;
    }

    if (normalized < -90) {
        normalized += 180;
    }

    return normalized;
}
