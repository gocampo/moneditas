FROM node:lts-alpine
ENV NODE_ENV=production
WORKDIR /usr/src
COPY ["package.json", "package-lock.json*", "npm-shrinkwrap.json*", "./"]
RUN npm install --production --silent 
COPY . .
RUN chown -R node /usr/src
RUN npm run build
CMD ["tail", "-f", "/dev/null"]