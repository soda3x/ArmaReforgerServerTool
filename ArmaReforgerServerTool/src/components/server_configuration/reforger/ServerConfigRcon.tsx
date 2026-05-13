import { Button } from "@/components/ui/button";
import { CircleCheck, CircleX } from "lucide-react";
import { ServerParameterBoolean, ServerParameterSelect, ServerParameterInput } from "@/components/server_configuration/controls";
import { Label } from "@/components/ui/label";

const RCON_PERMISSIONS: string[] = ["Monitor", "Admin"]

export default function ServerParamsRcon() {
  return (
    <>
      <Label className="pb-2 text-lg">RCON</Label>
      <div className="flex flex-col gap-4">
        <div className="flex justify-between">
          <ServerParameterBoolean id="enable-rcon" label="Enable RCON" />
          <ServerParameterSelect placeholder="Permission" label="RCON Permission" items={RCON_PERMISSIONS} />
        </div>
        <div className="flex items-center justify-between gap-2">
          <ServerParameterInput id="rcon-address" label="RCON Address" placeholder="127.0.0.1" />
          <ServerParameterInput id="rcon-port" label="RCON Port" defaultValue={19999} minValue={1025} maxValue={65335} type="number" />
        </div>
        <div className="flex items-center justify-between gap-2">
          <ServerParameterInput id="rcon-pw" label="RCON Password" placeholder="monitormyserver123" />
          <ServerParameterInput id="rcon-max-clients" label="Max Clients" defaultValue={16} type="number" />
        </div>

        <div className="flex items-center gap-2">
          <Button><CircleCheck /> Edit Whitelist</Button>
          <Button><CircleX /> Edit Blacklist</Button>
        </div>
      </div>
    </>
  );
}