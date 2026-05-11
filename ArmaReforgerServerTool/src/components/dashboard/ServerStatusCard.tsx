import { Card, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import MiniGraph, { DEBUG_MINIGRAPH_MOCK_DATA } from "@/components/common/MiniGraph";

export default function ServerStatusCard({ id, name, game, status, players, onClick }: any) {
  return (
    <Card 
      className="bg-card border-white/5 hover:border-brand-orange/50 hover:bg-white/[0.02] transition-all cursor-pointer group"
      onClick={() => onClick(id)}
    >
      <CardContent className="p-4">
        <div className="flex justify-between items-start mb-4">
          <div className="space-y-1">
            <h4 className="font-bold text-white group-hover:text-brand-orange transition-colors">{name}</h4>
            <p className="text-xs text-muted-foreground font-mono">{game}</p>
          </div>
          <Badge className={status === "online" ? "bg-green-500/10 text-green-500" : ""}>
            {status}
          </Badge>
        </div>
        <div className="mt-4 text-[10px] font-mono text-brand-orange opacity-0 group-hover:opacity-100 transition-opacity">
          Go to Server →
        </div>
      </CardContent>
      <div>
        <MiniGraph data={DEBUG_MINIGRAPH_MOCK_DATA}/>
      </div>
    </Card>
  );
}