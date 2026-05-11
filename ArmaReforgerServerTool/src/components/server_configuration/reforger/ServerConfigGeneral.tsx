import { FieldDescription, Field, FieldLabel, FieldGroup, FieldSet, FieldLegend } from "@/components/ui/field"
import { Input } from "@/components/ui/input"
import { Switch } from "@/components/ui/switch";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";
import { Xbox } from "@/components/icons/Xbox";
import { PlayStation } from "@/components/icons/PlayStation";

export default function ServerParamsGeneral() {
  return (
    <>
      <FieldGroup>
        <FieldSet>
          <FieldLegend>General</FieldLegend>
          {/* <FieldDescription>General Server Parameters</FieldDescription> */}
        </FieldSet>
      </FieldGroup>
      <div className="flex flex-col gap-4">
        <FieldGroup>
          <Field>
            <FieldLabel htmlFor="server-name">Server Name <span className="text-destructive">*</span></FieldLabel>
            <Input
              id="server-name"
              placeholder="My Longbow Arma Server"
              required
            />
          </Field>
        </FieldGroup>
        <div className="flex items-center justify-between gap-2">
        <FieldGroup>
          <Field>
            <FieldLabel htmlFor="server-pw">Server Password</FieldLabel>
            <Input
              id="server-pw"
              placeholder=""
            />
          </Field>
        </FieldGroup>

        <FieldGroup>
          <Field>
            <FieldLabel htmlFor="admin-pw">Admin Password</FieldLabel>
            <Input
              id="admin-pw"
              placeholder=""
            />
          </Field>
        </FieldGroup>
        </div>

        <FieldGroup>
          <Field>
            <FieldLabel htmlFor="max-players">Max Players</FieldLabel>
            <Input
              id="max-players"
              placeholder=""
              type="number"
            />
          </Field>
        </FieldGroup>

        <FieldGroup>
          <div className="flex justify-between">
            <div className="flex items-center space-x-2">
              <Label htmlFor="server-visible">Server Visible</Label>
              <Switch id="server-visible" />
            </div>
            <Badge>Required for discovery in Server Browser</Badge>
          </div>
        </FieldGroup>

        <FieldGroup>
          <div className="flex justify-between">
            <div className="flex items-center space-x-2">
              <Label htmlFor="cross-platform">Cross Platform</Label>
              <Switch id="cross-platform" />
            </div>
            <Badge>Enable to play with <Xbox/> and <PlayStation/> players</Badge>
          </div>
        </FieldGroup>
      </div>
    </>
  );
}