import { Component } from 'solid-js';
import { Flex } from '@hope-ui/solid';
import { TopNav } from "./TopNav";
import { ContentBody } from "./ContentBody";
import { SideNav } from "./SideNav";

export const Layout: Component = () => {
  return (
    <Flex flexDirection="column" flex={1}>
      <TopNav />
      <Flex direction="row" flex={1} bg="$blackAlpha7"> {/* left to right */}
        <SideNav />
        <ContentBody />
      </Flex>
    </Flex>
  );
};
