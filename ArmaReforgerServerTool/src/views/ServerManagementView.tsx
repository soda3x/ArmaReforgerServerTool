import MiniGraph, { DEBUG_MINIGRAPH_MOCK_DATA } from "@/components/MiniGraph";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Label } from "@/components/ui/label";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Terminal, Play, Square, Settings2, Clipboard, Info, Activity, Archive, Binoculars } from "lucide-react";

export default function ServerManagementView({ serverId }: { serverId: string }) {
  return (
    <div className="space-y-6 animate-in slide-in-from-right-4 duration-300">
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
        {/* Console Output */}
        <div className="col-span-3 space-y-2">
          <div className="flex items-center gap-2 text-xs font-mono text-muted-foreground">
            <Terminal className="h-3 w-3 text-brand-orange" /> LIVE_CONSOLE_OUTPUT
          </div>
          <div className="console-screen h-[800px] rounded-md overflow-y-auto border border-white/10 font-mono text-green-400">
            <p>[08:22:01] Initializing Game Server...</p>
            <p>[08:22:05] Loading Mods: @CBA_A3, @ACE...</p>
            <p>[08:22:10] Server identity verified.</p>
            <p className="text-brand-orange">[08:22:12] Mission: Combat_Op_Everon started.</p>
            <p className="animate-pulse">_</p>
          </div>
        </div>

        {/* Quick Settings */}
        <div className="space-y-4">
          <div className="flex items-center gap-2 text-xs font-mono text-muted-foreground uppercase tracking-widest">
            <Activity className="h-3 w-3 text-brand-orange" /> PERFORMANCE_METRICS
          </div>
          <Card className="bg-card border-white/5 overflow-hidden">
            <CardContent className="p-4 space-y-4">

              {/* CPU Usage */}
              <div className="space-y-4">
                <div className="flex justify-between text-[10px] font-mono uppercase">
                  <span className="text-muted-foreground">CPU Core Load</span>
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
                  <span className="text-muted-foreground">Allocated RAM</span>
                  <span className="text-white">4.2GB / 8GB</span>
                </div>
                <div className="h-1 w-full bg-white/5 rounded-full overflow-hidden">
                  <div className="h-full bg-brand-orange w-[52%] shadow-[0_0_8px_rgba(255,96,0,0.5)]" />
                </div>
                <MiniGraph data={DEBUG_MINIGRAPH_MOCK_DATA} />
              </div>
            </CardContent>
          </Card>

          <div className="flex items-center gap-2 col-start-3 row-start-1 text-xs font-mono text-muted-foreground">
            <Info className="h-3 w-3 text-brand-orange" /> SERVER_INFO
          </div>
          <Card className="bg-card border-white/5">
            <CardContent className="p-4 space-y-4">

              <div>
                <Label className="w-full text-gray-400 justify-start text-xs">Player Count</Label>
                <div className="flex items-justify">
                  <Label className="w-full justify-start text-lg">0/16 players</Label>
                  <Button size="sm" className="bg-brand-orange hover:bg-brand-orange/80"><Clipboard /></Button>
                </div>
              </div>

              <div>
                <Label className="w-full text-gray-400 justify-start text-xs">Address</Label>
                <div className="flex items-justify">
                  <Label className="w-full justify-start text-lg">127.0.0.1:2001</Label>
                  <Button size="sm" className="bg-brand-orange hover:bg-brand-orange/80"><Clipboard /></Button>
                </div>
              </div>
              <div>
                <Label className="w-full text-gray-400 justify-start text-xs">Ping Site</Label>
                <div className="flex items-justify">
                  <Label className="w-full justify-start text-lg">Sydney</Label>
                  <Button size="sm" className="bg-brand-orange hover:bg-brand-orange/80"><Clipboard /></Button>
                </div>
              </div>
              <div>
                <Label className="w-full text-gray-400 justify-start text-xs">Join Code</Label>
                <div className="flex items-justify">
                  <Label className="w-full justify-start text-lg">12345678</Label>
                  <Button size="sm" className="bg-brand-orange hover:bg-brand-orange/80"><Clipboard /></Button>
                </div>
              </div>
            </CardContent>
          </Card>
          <div className="flex items-center gap-2 col-start-3 row-start-2 text-xs font-mono text-muted-foreground">
            <Settings2 className="h-3 w-3 text-brand-orange" /> QUICK_ACTIONS
          </div>
          <Card className="bg-card border-white/5">
            <CardContent className="p-4 space-y-4">
              <Button variant="ghost" className="w-full justify-start text-xs"><Binoculars/>Open in ReCON</Button>
              <Button variant="ghost" className="w-full justify-start text-xs"><Archive/>Manage Backups</Button>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
}