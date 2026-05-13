import { Textarea } from "@/components/ui/textarea";

type TextAreaProps = {
  id: string,
  placeholder?: string,
  defaultValue?: string
}
export default function ServerParameterTextArea({ id, placeholder, defaultValue }: TextAreaProps) {
  return (
    <Textarea className="h-full resize-none overflow-y-auto" id={id} placeholder={placeholder} defaultValue={defaultValue} />
  );
}