import { Cog } from "lucide-react";
import { Card, CardContent } from "@/components/ui/card";
import { ServerParamsGeneral, ServerParamsNetwork, ServerParamsRcon, ServerParamsGameProperties, ServerParamsPersistence, ServerParamsOperating } from "@/components/server_configuration/reforger";

export default function ServerConfig() {
  return (
    <div className="flex flex-col flex-1 h-full col-span-3 space-y-4">
      <div className="flex items-center gap-2 text-xs font-mono text-muted-foreground">
        <Cog className="h-3 w-3 text-brand-orange" /> SERVER_CONFIGURATION
      </div>
      <div className="flex flex-col flex-1 h-full grid grid-cols-2 gap-4">
        <Card className="bg-card border-white/5">
          <CardContent>
            <ServerParamsGeneral />
          </CardContent>
        </Card>

        <Card className="bg-card border-white/5">
          <CardContent>
            <ServerParamsNetwork />
          </CardContent>
        </Card>

        <Card className="bg-card border-white/5 flex flex-col">
          <CardContent className="flex-1 flex flex-col">
            <ServerParamsOperating />
          </CardContent>
        </Card>

        <Card className="bg-card border-white/5">
          <CardContent>
            <ServerParamsRcon />
          </CardContent>
        </Card>

        <Card className="bg-card border-white/5">
          <CardContent>
            <ServerParamsGameProperties />
          </CardContent>
        </Card>

        <Card className="bg-card border-white/5">
          <CardContent>
            <ServerParamsPersistence />
          </CardContent>
        </Card>
      </div>
    </div>
  );
}