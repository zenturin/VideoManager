import { useEffect, useState } from "react";
import { useApi, type RepoGraphData } from "./ServerWrapper";
import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, Cell } from "recharts";
import "./RepoGraphs.css";

type Props = {
    repoId: number | null;
}

type GraphKey = "duration" | "size" | "year";

const GRAPHS: { key: GraphKey; label: string }[] = [
    { key: "duration", label: "Duration" },
    { key: "size",     label: "File Size" },
    { key: "year",     label: "Year" },
];

export default function RepoGraphs({ repoId }: Props) {
    const api = useApi();
    const [graphData, setGraphData] = useState<RepoGraphData | null>(null);
    const [loading, setLoading] = useState(false);
    const [activeGraph, setActiveGraph] = useState<GraphKey>("duration");

    useEffect(() => {
        if (repoId == null) return;
        setGraphData(null);
        setLoading(true);
        api.getRepoGraphs(repoId)
            .then(data => { setGraphData(data); setLoading(false); })
            .catch(() => setLoading(false));
    }, [repoId]);

    if (repoId == null) return null;

    const chartData = graphData
        ? activeGraph === "duration" ? graphData.DurationHistogram
        : activeGraph === "size"     ? graphData.SizeHistogram
        :                              graphData.YearHistogram
        : [];

    return (
        <div className="repo-graphs">
            <h3>Graphs</h3>
            <div className="graph-tabs">
                {GRAPHS.map(g => (
                    <button
                        key={g.key}
                        className={`graph-tab${activeGraph === g.key ? " active" : ""}`}
                        onClick={() => setActiveGraph(g.key)}
                    >
                        {g.label}
                    </button>
                ))}
            </div>
            <div className="graph-area">
                {loading && <p className="graph-loading">Loading graph data…</p>}
                {!loading && graphData && (
                    <ResponsiveContainer width="100%" height={220}>
                        <BarChart data={chartData} margin={{ top: 8, right: 16, left: 0, bottom: 40 }}>
                            <XAxis
                                dataKey="Bucket"
                                tick={{ fill: "wheat", fontSize: 11 }}
                                angle={-35}
                                textAnchor="end"
                                interval={0}
                            />
                            <YAxis tick={{ fill: "wheat", fontSize: 11 }} allowDecimals={false} />
                            <Tooltip
                                contentStyle={{ backgroundColor: "#555", border: "1px solid black", color: "wheat" }}
                                cursor={{ fill: "rgba(255,255,255,0.1)" }}
                            />
                            <Bar dataKey="Count" radius={[3, 3, 0, 0]}>
                                {chartData.map((_, i) => (
                                    <Cell key={i} fill={`hsl(${200 + i * 20}, 60%, 55%)`} />
                                ))}
                            </Bar>
                        </BarChart>
                    </ResponsiveContainer>
                )}
            </div>
        </div>
    );
}
