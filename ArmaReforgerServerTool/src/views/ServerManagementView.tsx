import Console from "@/components/server_management/Console";
import PerfMetrics from "@/components/server_management/PerfMetrics";
import QuickActions from "@/components/server_management/QuickActions";
import ServerConfig from "@/components/server_management/ServerConfig";
import ServerInfo from "@/components/server_management/ServerConfig";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Play, Square } from "lucide-react";

export default function ServerManagementView({ serverId }: { serverId: string }) {
  return (
    <div className="space-y-6 animate-in slide-in-from-right-4 duration-300 pb-10">
      <div className="flex justify-between items-center">
        <div>
          <h2 className="text-2xl font-bold text-white">{serverId.toUpperCase()}</h2>
          <div className="flex gap-2">
            <Badge className="bg-green-400 text-black">ONLINE</Badge>
            <Badge className="bg-gray-600">Uptime: 14h 22m</Badge>
            <Badge>Arma Reforger</Badge>
          </div>
        </div>
        <div className="flex gap-2">
          <Button variant="outline" size="sm" className="border-red-500/50 text-red-500 hover:bg-red-500/10">
            <Square className="h-4 w-4 mr-2" /> Stop
          </Button>
          <Button size="sm" className="bg-brand-orange hover:bg-brand-orange/80">
            <Play className="h-4 w-4 mr-2" /> Restart
          </Button>
        </div>
      </div>

      <div className="grid grid-cols-4 gap-6">
        {/* Main Panel */}
        <Console />
        <ServerConfig/>

        {/* Right Sidebar */}
        <div className="space-y-4 col-start-4 row-start-1">
          <PerfMetrics />
          <ServerInfo />
          <QuickActions />
        </div>
      </div>
    </div>
  );
}