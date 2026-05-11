import { Cog } from "lucide-react";
import { Card, CardContent } from "@/components/ui/card";
import ServerParamsGeneral from "@/components/server_configuration/reforger/ServerConfigGeneral";
import ServerParamsNetwork from "@/components/server_configuration/reforger/ServerConfigNetwork";
import ServerParamsRcon from "@/components/server_configuration/reforger/ServerConfigRcon";
import ServerParamsGameProperties from "../server_configuration/reforger/ServerConfigGameProperties";

export default function ServerConfig({ ...params }) {
  return (
    <div className={`col-span-3 space-y-4 ${params}`}>
      <div className="flex items-center gap-2 text-xs font-mono text-muted-foreground">
        <Cog className="h-3 w-3 text-brand-orange" /> SERVER_CONFIGURATION
      </div>
      <div className="flex flex-col grid grid-cols-2 gap-4">
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
      </div>
    </div>
  );
}