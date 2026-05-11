import { FieldDescription, Field, FieldLabel, FieldGroup, FieldSet, FieldLegend } from "@/components/ui/field"
import { Input } from "@/components/ui/input"
import { Switch } from "@/components/ui/switch";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";

export default function ServerParamsGameProperties() {
  return (
    <>
      <FieldGroup>
        <FieldSet>
          <FieldLegend>Game Properties</FieldLegend>
          {/* <FieldDescription>General Server Parameters</FieldDescription> */}
        </FieldSet>
      </FieldGroup>
      <div className="flex flex-col gap-4">
        <div className="flex items-center justify-between gap-2">
          <FieldGroup>
            <Field>
              <FieldLabel htmlFor="server-max-view-distance">Server Max View Distance</FieldLabel>
              <Input
                id="server-max-view-distance"
                type="number"
              />
            </Field>
          </FieldGroup>

          <FieldGroup>
            <Field>
              <FieldLabel htmlFor="network-view-distance">Network View Distance</FieldLabel>
              <Input
                id="network-view-distance"
                type="number"
              />
            </Field>
          </FieldGroup>
        </div>
        <div className="flex items-center justify-between gap-2">
          <FieldGroup>
            <div className="flex justify-between">
              <div className="flex items-center space-x-2">
                <Label htmlFor="disable-third-person">Disable Third Person</Label>
                <Switch id="disable-third-person" />
              </div>
              {/* <Badge>Required for discovery in Server Browser</Badge> */}
            </div>
          </FieldGroup>


          <FieldGroup>
            <div className="flex justify-between">
              <div className="flex items-center space-x-2">
                <Label htmlFor="fast-validation">Fast Validation</Label>
                <Switch id="fast-validation" />
              </div>
              <Badge>Recommended</Badge>
            </div>
          </FieldGroup>
        </div>
        <div className="flex items-center justify-between gap-2">
          <FieldGroup>
            <div className="flex justify-between">
              <div className="flex items-center space-x-2">
                <Label htmlFor="battleye">BattlEye Anti-cheat</Label>
                <Switch id="battleye" />
              </div>
              {/* <Badge>Recommended</Badge> */}
            </div>
          </FieldGroup>
          <FieldGroup>
            <div className="flex justify-between">
              <div className="flex items-center space-x-2">
                <Label htmlFor="lobby-player-synchronise">Lobby Player Synchronise</Label>
                <Switch id="lobby-player-synchronise" />
              </div>
              {/* <Badge>Recommended</Badge> */}
            </div>
          </FieldGroup>
        </div>
        <div className="flex items-center justify-between gap-2">
          <FieldGroup>
            <div className="flex justify-between">
              <div className="flex items-center space-x-2">
                <Label htmlFor="von-disable-ui">VON Disable UI</Label>
                <Switch id="von-disable-ui" />
              </div>
              {/* <Badge>Recommended</Badge> */}
            </div>
          </FieldGroup>
          <FieldGroup>
            <div className="flex justify-between">
              <div className="flex items-center space-x-2">
                <Label htmlFor="von-disable-direct-speech-ui">VON Disable Direct Speech UI</Label>
                <Switch id="von-disable-direct-speech-ui" />
              </div>
              {/* <Badge>Recommended</Badge> */}
            </div>
          </FieldGroup>
          </div>
          <FieldGroup>
            <div className="flex justify-between">
              <div className="flex items-center space-x-2">
                <Label htmlFor="von-can-transmit-cross-faction">VON Can Transmit Cross Faction</Label>
                <Switch id="von-can-transmit-cross-faction" />
              </div>
              {/* <Badge>Recommended</Badge> */}
            </div>
          </FieldGroup>
        </div>
    </>
  );
}