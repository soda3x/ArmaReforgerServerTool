import { ServerParameterInput } from "../controls";
import { Button } from "@/components/ui/button";
import { Database, HardDrive } from "lucide-react";
import { Label } from "@/components/ui/label";

export default function ServerParamsPersistence() {
  return (
    <>
      <Label className="pb-2 text-lg">Persistence</Label>
      <div className="flex flex-col gap-4">
        <div className="flex items-center justify-between gap-2">
          <ServerParameterInput id="autoSaveInterval" label="Autosave Interval" placeholder="10" defaultValue={10} minValue={0} maxValue={60} type="number" />
          <ServerParameterInput id="hiveId" label="Hive ID" placeholder="0" defaultValue={0} minValue={0} maxValue={16383} type="number" />
        </div>
        <div className="flex items-center gap-2">
          <Button><Database /> Edit Databases</Button>
          <Button><HardDrive /> Edit Storages</Button>
        </div>
      </div>
    </>
  );
}