import { FieldDescription, Field, FieldLabel, FieldGroup, FieldSet, FieldLegend } from "@/components/ui/field"
import { Input } from "@/components/ui/input"
import { Switch } from "@/components/ui/switch";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";
import { Select, SelectContent, SelectGroup, SelectItem, SelectLabel, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Button } from "@/components/ui/button";
import { List } from "lucide-react";
import { Dialog } from "@/components/ui/dialog";

export default function ServerParamsRcon() {
  return (
    <>
      <FieldGroup>
        <FieldSet>
          <FieldLegend>RCON</FieldLegend>
          {/* <FieldDescription>General Server Parameters</FieldDescription> */}
        </FieldSet>
      </FieldGroup>
      <div className="flex flex-col gap-4">
        <FieldGroup>
          <div className="flex justify-between">
            <div className="flex items-center space-x-2">
              <Label htmlFor="enable-rcon">Enable RCON</Label>
              <Switch id="enable-rcon" />
            </div>
            <Select>
              <SelectTrigger className="w-full max-w-48">
                <SelectValue placeholder="Permission" />
              </SelectTrigger>
              <SelectContent>
                <SelectGroup>
                  <SelectLabel>RCON Permission</SelectLabel>
                  <SelectItem value="Monitor">Monitor</SelectItem>
                  <SelectItem value="Admin">Admin</SelectItem>
                </SelectGroup>
              </SelectContent>
            </Select>
          </div>
        </FieldGroup>
        <div className="flex items-center justify-between gap-2">
          <FieldGroup>
            <Field>
              <FieldLabel htmlFor="rcon-address">RCON Address</FieldLabel>
              <Input
                id="rcon-address"
                placeholder="127.0.0.1"
              />
            </Field>
          </FieldGroup>

          <FieldGroup>
            <Field>
              <FieldLabel htmlFor="rcon-port">RCON Port</FieldLabel>
              <Input
                id="rcon-port"
                value="19999"
                type="number"
              />
            </Field>
          </FieldGroup>
        </div>
        <div className="flex items-center justify-between gap-2">
          <FieldGroup>
            <Field>
              <FieldLabel htmlFor="rcon-pw">RCON Password</FieldLabel>
              <Input
                id="rcon-pw"
                placeholder="monitormyserver123"
              />
            </Field>
          </FieldGroup>

          <FieldGroup>
            <Field>
              <FieldLabel htmlFor="rcon-max-clients">Max Clients</FieldLabel>
              <Input
                id="rcon-max-clients"
                value="16"
                type="number"
              />
            </Field>
          </FieldGroup>
        </div>

        <div className="flex items-center gap-2">
          <Button><List /> Edit Whitelist</Button>
          <Button><List /> Edit Blacklist</Button>
        </div>
      </div>
    </>
  );
}