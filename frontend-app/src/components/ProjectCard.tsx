import { Box, Flex, Center, Button } from "@hope-ui/solid"
import { Link } from "@solidjs/router"
import { Component } from "solid-js"

export type ProjectProps = {
  id: number,
  name: string,
  tasks: number,
}

export const ProjectCard: Component<ProjectProps> = (props: ProjectProps) => {
  return (
    <Box
      width="100%"
      borderWidth="1px"
      borderColor="$neutral6"
      borderRadius="$lg"
      overflow="hidden"
    >
      <Flex>
        <Flex direction="column">
          <Box p="$6">
            <Box fontWeight="$semibold" as="h4" lineHeight="$tight" noOfLines={1}>
              {props.name}
            </Box>
          </Box>
          <Box px="$6" pb="$6">
            <Box fontWeight="$normal" as="h4" lineHeight="$normal" noOfLines={1}>
              Tasks: {props.tasks}
            </Box>
          </Box>
        </Flex>
        <Center ml="auto">
          <Box p="$6">
            <Button as={Link} href={`/projects/${props.id}/details`}>View</Button>
          </Box>
        </Center>
      </Flex>
    </Box>
  )
}
