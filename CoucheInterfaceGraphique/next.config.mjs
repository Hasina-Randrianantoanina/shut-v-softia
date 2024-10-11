/** @type {import('next').NextConfig} */
const nextConfig = {
    async rewrites() {
        return [
            {
                source: '/api/:path*',
                destination: 'http://localhost:5154/:path*',
            },
            {
                source: '/api/:path*',
                destination: 'http://localhost:5194/:path*',
            },
            {
                source: '/timescale/:path*',
                destination: 'http://localhost:5228/:path*',
            },
        ];
    },
    async redirects() {
        return [
            {
                source: '/',
                destination: '/login',
                permanent: false,
            },
        ];
    },
    reactStrictMode: false,
};
export default nextConfig;
