import { useEffect, useState } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { RefreshCw, Server, Shield, CheckCircle2, AlertCircle } from "lucide-react";
import ServerStatusCard from "@/components/ServerStatusCard";

interface DashboardProps {
  onSelectServer: (id: string) => void;
}

export function DashboardView({ onSelectServer }: DashboardProps) {
  const [updateStatus, setUpdateStatus] = useState<"checking" | "up-to-date">("checking");

  // Simulate an update check on launch
  useEffect(() => {
    const timer = setTimeout(() => setUpdateStatus("up-to-date"), 3000);
    return () => clearTimeout(timer);
  }, []);

  return (
    <div className="space-y-6 animate-in fade-in duration-500">
      {/* Top Row: System Health & Updates */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Card className="tactical-glass border-brand-orange/20">
          <CardHeader className="flex flex-row items-center justify-between pb-2">
            <CardTitle className="text-sm font-mono uppercase tracking-tighter">
              System Orchestrator
            </CardTitle>
            <Shield className="h-4 w-4 text-brand-orange" />
          </CardHeader>
          <CardContent>
            <div className="flex items-center gap-3">
              {updateStatus === "checking" ? (
                <>
                  <RefreshCw className="h-5 w-5 animate-spin text-brand-orange" />
                  <p className="text-sm font-medium">Checking for latest server binaries...</p>
                </>
              ) : (
                <>
                  <CheckCircle2 className="h-5 w-5 text-green-500 status-glow-green" />
                  <div>
                    <p className="text-sm font-medium text-white">Longbow Core v2.0.4</p>
                    <p className="text-xs text-muted-foreground">All systems nominal. No updates required.</p>
                  </div>
                </>
              )}
            </div>
          </CardContent>
        </Card>

        <Card className="bg-card/40 border-border">
          <CardHeader className="flex flex-row items-center justify-between pb-2">
            <CardTitle className="text-sm font-mono uppercase tracking-tighter text-muted-foreground">
              Global Resource Load
            </CardTitle>
            <Server className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
             <div className="flex justify-between items-end">
                <div>
                  <p className="text-2xl font-bold text-white">12.4<span className="text-xs font-normal text-muted-foreground ml-1">GB</span></p>
                  <p className="text-xs text-muted-foreground">Physical Memory in Use</p>
                </div>
                <div className="text-right">
                  <p className="text-2xl font-bold text-white">4.2<span className="text-xs font-normal text-muted-foreground ml-1">%</span></p>
                  <p className="text-xs text-muted-foreground">Global CPU Load</p>
                </div>
             </div>
          </CardContent>
        </Card>
      </div>

      {/* Active Servers Grid */}
      <div>
        <h3 className="text-xs font-mono uppercase tracking-[0.2em] text-muted-foreground mb-4 ml-1">
          Active Deployments
        </h3>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <ServerStatusCard onClick={onSelectServer} name="EU-Alpha-Reforger" game="Reforger" status="online" players="42/64" />
          <ServerStatusCard onClick={onSelectServer} name="US-East-Arma3" game="Arma 3" status="online" players="12/100" />
          <ServerStatusCard onClick={onSelectServer} name="Dev-Sandbox-4" game="Arma 4" status="offline" players="0/0" />
        </div>
      </div>
    </div>
  );
}