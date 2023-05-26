import { Container, Heading } from "@hope-ui/solid";
import { Component } from "solid-js";

const Home: Component = () => {
  return (
    <Container p="$2">
      <Heading size="xl">
        Home
      </Heading>
      <Container mt="$4">
        This is a simple project management app where you can add projects and todo items.
      </Container>
    </Container>
  );
}

export default Home;
