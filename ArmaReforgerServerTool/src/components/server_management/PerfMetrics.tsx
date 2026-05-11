import { Activity } from "lucide-react";
import MiniGraph, { DEBUG_MINIGRAPH_MOCK_DATA } from "@/components/common/MiniGraph";
import { Card, CardContent } from "@/components/ui/card";

export default function PerfMetrics() {
  return (
    <>
      <div className="flex items-center gap-2 text-xs font-mono text-muted-foreground uppercase tracking-widest">
        <Activity className="h-3 w-3 text-brand-orange" /> PERFORMANCE_METRICS
      </div>
      <Card className="bg-card border-white/5 overflow-hidden">
        <CardContent className="p-4 space-y-4">

          {/* CPU Usage */}
          <div className="space-y-4">
            <div className="flex justify-between text-[10px] font-mono uppercase">
              <span className="text-muted-foreground">CPU Usage</span>
              <span className="text-white">18.4%</span>
            </div>
            <div className="h-1 w-full bg-white/5 rounded-full overflow-hidden">
              <div className="h-full bg-brand-orange w-[18%] shadow-[0_0_8px_rgba(255,96,0,0.5)]" />
            </div>
            <MiniGraph data={DEBUG_MINIGRAPH_MOCK_DATA} />
          </div>

          {/* Memory Usage */}
          <div className="space-y-4">
            <div className="flex justify-between text-[10px] font-mono uppercase">
              <span className="text-muted-foreground">RAM Usage</span>
              <span className="text-white">4.2GB / 8GB</span>
            </div>
            <div className="h-1 w-full bg-white/5 rounded-full overflow-hidden">
              <div className="h-full bg-brand-orange w-[52%] shadow-[0_0_8px_rgba(255,96,0,0.5)]" />
            </div>
            <MiniGraph data={DEBUG_MINIGRAPH_MOCK_DATA} />
          </div>
        </CardContent>
      </Card>
    </>
  );
}