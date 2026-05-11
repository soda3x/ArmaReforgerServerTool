import { Archive, Binoculars, Settings2 } from "lucide-react";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";


export default function QuickActions() {
  return (
    <>
      <div className="flex items-center gap-2 col-start-3 row-start-2 text-xs font-mono text-muted-foreground">
        <Settings2 className="h-3 w-3 text-brand-orange" /> QUICK_ACTIONS
      </div>
      <Card className="bg-card border-white/5">
        <CardContent className="p-4 space-y-4">
          <Button variant="ghost" className="w-full justify-start text-xs"><Binoculars />Open in ReCON</Button>
          <Button variant="ghost" className="w-full justify-start text-xs"><Archive />Manage Backups</Button>
        </CardContent>
      </Card>
    </>
  );
}