import { Icon } from "@hope-ui/solid";
import { FiEdit2 } from 'solid-icons/fi';
import { AiOutlineDelete } from 'solid-icons/ai'

const IconEdit = () => {
  return <Icon as={FiEdit2} />
}

const IconDelete = () => {
  return <Icon as={AiOutlineDelete} />
}

export { IconEdit, IconDelete }
