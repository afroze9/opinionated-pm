import { JSX } from 'solid-js';
import { Anchor, Box, SystemStyleObject } from '@hope-ui/solid';
import { Link } from '@solidjs/router';
import { SideNavLinkProps } from './SideNav';

const boxStyle: SystemStyleObject = {
  width: '100%',
  padding: '$2',
  _hover: {
    background: '$accent8'
  }
};

const anchorStyle: SystemStyleObject = {
  width: '100%',
  _hover: {
    textDecoration: 'none'
  }
};

export const SideNavLink = (props: SideNavLinkProps): JSX.Element => {
  return (
    <Anchor
      as={Link}
      href={props.link.path}
      color="$accent1"
      css={anchorStyle}
    >
      <Box css={boxStyle}>
        {props.link.name}
      </Box>
    </Anchor>
  );
};
