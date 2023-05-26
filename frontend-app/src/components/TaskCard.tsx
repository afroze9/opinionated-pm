import { Box, Flex, Center, Button } from "@hope-ui/solid"
import { Component } from "solid-js"

export type TaskProps = {
  id: number,
  title: string,
  description: string,
  isCompleted: boolean,
  onComplete: (taskId: number) => Promise<void>
}

export const TaskCard: Component<TaskProps> = (props: TaskProps) => {
  return (
    <Box
      width="100%"
      borderWidth="1px"
      borderColor="$neutral6"
      borderRadius="$lg"
      overflow="hidden"
    >
      <Flex>
        <Box p="$6">
          <Box fontWeight="$semibold" as="h4" textDecoration={props.isCompleted ? "line-through" : "none"} lineHeight="$tight" noOfLines={1}>
            {props.title}
          </Box>
          <Box fontWeight="$normal" as="p" textDecoration={props.isCompleted ? "line-through" : "none"} lineHeight="$tight" noOfLines={3}>
            {props.description}
          </Box>
        </Box>
        <Center ml="auto">
          <Box p="$6">
            <Button disabled={props.isCompleted} onClick={() => props.onComplete(props.id)}>Complete</Button>
          </Box>
        </Center>
      </Flex>
    </Box>
  )
}
