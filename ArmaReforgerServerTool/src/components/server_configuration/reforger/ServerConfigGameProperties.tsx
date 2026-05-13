import { ServerParameterBoolean, ServerParameterInput } from "@/components/server_configuration/controls";
import { Label } from "@/components/ui/label";

export default function ServerParamsGameProperties() {
  return (
    <>
      <Label className="pb-2 text-lg">Game Properties</Label>
      <div className="flex flex-col gap-4">
        <div className="flex items-center justify-between gap-2">
          <ServerParameterInput id="server-max-view-distance" label="Server Max View Distance" minValue={500} maxValue={10000} defaultValue={1600} type="number" />
          <ServerParameterInput id="server-min-grass-distance" label="Server Min Grass Distance" minValue={0} maxValue={150} defaultValue={0} type="number" hint="0 means no distance is forced upon clients" />
        </div>
        <ServerParameterInput id="network-view-distance" label="Network View Distance" minValue={500} maxValue={5000} defaultValue={1500} type="number" />
        <div className="flex items-center justify-between gap-2">
          <ServerParameterBoolean id="von-disable-ui" label="VON Disable UI" />
          <ServerParameterBoolean id="von-disable-direct-speech-ui" label="VON Disable Direct Speech UI" />
        </div>
        <ServerParameterBoolean id="von-can-transmit-cross-faction" label="VON Transmit Cross Faction" />
      </div>
    </>
  );
}