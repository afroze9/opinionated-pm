function getInitials(name: string): string {
	const words = name.trim().split(/\s+/);
	let initials = words.length > 0 ? words[0].charAt(0) : '';

	if (words.length > 1) {
		initials += words[1].charAt(0);
	}

	return initials.toUpperCase();
}

const Helpers = {
	getInitials
};

export default Helpers;
