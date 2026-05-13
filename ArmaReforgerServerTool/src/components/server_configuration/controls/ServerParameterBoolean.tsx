import { Label } from "@/components/ui/label";
import { Switch } from "@/components/ui/switch";
import { Dispatch, ReactNode, SetStateAction } from "react";

type BooleanProps = {
  label: string,
  id: string,
  startState?: boolean,
  onSwitchChange?: Dispatch<SetStateAction<boolean>>;
  hint?: ReactNode
}
export default function ServerParameterBoolean({ id, label, startState = false, onSwitchChange = () => { }, hint }: BooleanProps) {
  return (
    <div className="flex items-center gap-4">
      <div className="flex items-center gap-4">
        <Label className="w-52" htmlFor={id}>{label}</Label>
        <Switch id={id} defaultChecked={startState} onCheckedChange={onSwitchChange} />
      </div>
      <Label className="hidden min-[1920px]:flex w-64 text-xs text-muted-foreground">{hint}</Label>
    </div>
  );
}