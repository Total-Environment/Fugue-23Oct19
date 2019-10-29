import chai, {expect} from 'chai';
import React from 'react';
import {shallow} from 'enzyme';
import { Content } from '../../src/components/checklists/content';

describe('Content', () => {
    let props;
    let wrapper;

    beforeEach(() => {
        props = {
            entries: [{
                cells: [{
                    key: '0 0',
                    value: 'S.No'
                }],
                key: 0
            },
                {
                    cells: [{
                        key: '1 0',
                        value: '1'
                    }],
                    key: 1
                }]
        };

        wrapper = shallow(<Content {...props}/>);
    });

    it('renderCell should return cell value with type td', () => {
        const result = wrapper.instance().renderCell(props.entries[0].cells[0]);
        expect(result.type).to.equal('td');
        expect(result.props.children).to.equal('S.No');
    });

    it('renderHeader should return cell value with type th', () => {
        const result = wrapper.instance().renderHead(props.entries[0].cells[0]);
        expect(result.type).to.equal('th');
        expect(result.props.children).to.equal('S.No');
    });

    it('renderEntry should return list of cells', () => {
        const result = wrapper.instance().renderEntry(props.entries[0]);
        expect(result.type).to.equal('tr');
        expect(result.props.children[0].props.children).to.equal('S.No');
        expect(result.props.children[0].type).to.equal('td');
    });


});
