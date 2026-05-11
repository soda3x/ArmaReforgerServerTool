import { FieldDescription, Field, FieldLabel, FieldGroup, FieldSet, FieldLegend } from "@/components/ui/field"
import { Input } from "@/components/ui/input"
import { Switch } from "@/components/ui/switch";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";

export default function ServerParamsNetwork() {
  return (
    <>
      <FieldGroup>
        <FieldSet>
          <FieldLegend>Network</FieldLegend>
          {/* <FieldDescription>General Server Parameters</FieldDescription> */}
        </FieldSet>
      </FieldGroup>
      <div className="flex flex-col gap-4">
        <div className="flex items-center justify-between gap-2">
          <FieldGroup>
            <Field>
              <FieldLabel htmlFor="bind-address">Bind Address</FieldLabel>
              <Input
                id="bind-address"
                value="0.0.0.0"
              />
            </Field>
          </FieldGroup>

          <FieldGroup>
            <Field>
              <FieldLabel htmlFor="bind-port">Bind Port</FieldLabel>
              <Input
                id="bind-port"
                value="2001"
                type="number"
              />
            </Field>
          </FieldGroup>
        </div>
        <div className="flex items-center justify-between gap-2">
          <FieldGroup>
            <Field>
              <FieldLabel htmlFor="public-address">Public Address</FieldLabel>
              <Input
                id="public-address"
                placeholder=""
              />
            </Field>
          </FieldGroup>

          <FieldGroup>
            <Field>
              <FieldLabel htmlFor="public-port">Public Port</FieldLabel>
              <Input
                id="public-port"
                placeholder=""
                type="number"
              />
            </Field>
          </FieldGroup>
        </div>
        <div className="flex items-center justify-between gap-2">
          <FieldGroup>
            <Field>
              <FieldLabel htmlFor="a2s-address">A2S Address</FieldLabel>
              <Input
                id="a2s-address"
                value="0.0.0.0"
              />
            </Field>
          </FieldGroup>

          <FieldGroup>
            <Field>
              <FieldLabel htmlFor="a2s-port">A2S Port</FieldLabel>
              <Input
                id="a2s-port"
                value="17777"
                type="number"
              />
            </Field>
          </FieldGroup>
        </div>
      </div>
    </>
  );
}