import { ServerParameterBoolean, ServerParameterInput } from "@/components/server_configuration/controls";
import { Label } from "@/components/ui/label";

export default function ServerParamsNetwork() {
  return (
    <>
      <Label className="pb-2 text-lg">Network</Label>
      <div className="flex flex-col gap-4">
        <div className="flex items-center justify-between gap-2">
          <ServerParameterInput id="bind-address" label="Bind Address" placeholder="0.0.0.0" defaultValue="0.0.0.0" />
          <ServerParameterInput id="bind-port" label="Bind Port" placeholder="2001" defaultValue="2001" type="number" minValue={1} maxValue={65335} />
        </div>
        <div className="flex items-center justify-between gap-2">
          <ServerParameterInput id="public-address" label="Public Address" placeholder="Leave blank to auto-detect" />
          <ServerParameterInput id="public-port" label="Public Port" placeholder="2001" defaultValue="2001" type="number" minValue={1} maxValue={65335} />
        </div>
        <div className="flex items-center justify-between gap-2">
          <ServerParameterInput id="a2s-address" label="A2S Address" placeholder="0.0.0.0" defaultValue="0.0.0.0" />
          <ServerParameterInput id="a2s-port" label="A2S Port" placeholder="17777" defaultValue="17777" type="number" minValue={1} maxValue={65335} />
        </div>
        <ServerParameterBoolean id="use-upnp" label="Use UPnP to open ports" hint="Port forwarding not required if this is enabled" />
      </div>
    </>
  );
}