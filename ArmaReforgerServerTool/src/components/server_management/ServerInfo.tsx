import { Activity, Clipboard } from "lucide-react";
import { Card, CardContent } from "@/components/ui/card";
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";

export default function PerfMetrics() {
  return (
    <>
      <div className="flex items-center gap-2 text-xs font-mono text-muted-foreground uppercase tracking-widest">
        <Activity className="h-3 w-3 text-brand-orange" /> SERVER_INFO
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
    </>
  );
}